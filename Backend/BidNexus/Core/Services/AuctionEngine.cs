using System.Collections.Concurrent;
using Core.Abstraction.AuctionRelated;
using Core.Abstraction.Services;
using Core.Entities.Auction;
using Core.Enumeration;
using Core.Exceptions;
using Core.Models.AuctionRelated;

namespace Core.Services;

public sealed class AuctionEngine(IAuctionRepository auctionRepository, IBidRepository bidRepository) : IAuctionEngine
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IBidRepository _bidRepository = bidRepository;

    private static readonly ConcurrentDictionary<int, SemaphoreSlim> _auctionLocks = new();

    public async Task<BidResponseDataModel> ProcessBidAsync(
        Bid request,
        CancellationToken cancellationToken = default)
    {
        if (request.AuctionId <= 0)
            throw new ArgumentException("AuctionId must be greater than zero.", nameof(request));

        if (request.VendorId <= 0)
            throw new ArgumentException("VendorId must be greater than zero.", nameof(request));

        var auctionLock = _auctionLocks.GetOrAdd(
            request.AuctionId,
            static _ => new SemaphoreSlim(1, 1));

        await auctionLock.WaitAsync(cancellationToken);
        try
        {
            var auction = await _auctionRepository.GetById(request.AuctionId);
            if (auction == null)
                throw new ValidationException("Auction was not found.");

            var now = DateTimeOffset.UtcNow;

            if (now < auction.AuctionStartTime)
                throw new ValidationException("Auction has not started yet.");

            if (now >= auction.AuctionEndTime)
                throw new ValidationException("Auction has already ended.");

            if (auction.StatusId != (short)StatusEnum.Open)
                throw new ValidationException("Auction is not open for bidding.");

            var leadingBid = await _bidRepository.GetLeadingBidForAuctionAsync(auction.Id, auction.IsForwardAuction);

            var validationErrors = IsValidBid(auction, leadingBid, request);
            if (validationErrors.Count > 0)
                throw validationErrors[0];

            var currentVendorBid = await _bidRepository.GetVendorsCurrentBidAsync(request.VendorId, request.AuctionId);

            return await _bidRepository.ProcessBidAsync(request, currentVendorBid?.Id, cancellationToken);
        }
        finally
        {
            auctionLock.Release();
        }
    }

    public List<ValidationException> IsValidBid(AuctionDataModel auction, BidDataModel? leadingBid, Bid request)
    {
        var validationErrors = new List<ValidationException>();

        if (leadingBid == null)
        {
            return validationErrors;
        }

        // 1. Overall Bid NetAmount Validation
        if (auction.IsForwardAuction && request.NetAmount <= leadingBid.NetAmount)
        {
            validationErrors.Add(new ValidationException($"Bid amount must be greater than the current leading bid of {leadingBid.NetAmount:0.00}."));
            return validationErrors;
        }
        else if (!auction.IsForwardAuction && request.NetAmount >= leadingBid.NetAmount)
        {
            validationErrors.Add(new ValidationException($"Bid amount must be less than the current leading bid of {leadingBid.NetAmount:0.00}."));
            return validationErrors;
        }

        // 2. Item-by-Item NetAmount Validation (matched by AuctionRequirementId)
        if (leadingBid.BidDetails != null && leadingBid.BidDetails.Count > 0)
        {
            var leadingItemMap = leadingBid.BidDetails.ToDictionary(x => x.AuctionRequirementId);
            var requirementNameMap = (auction.AuctionRequirements ?? [])
                .ToDictionary(r => r.Id, r => r.Item?.Name ?? $"Item #{r.Id}");

            foreach (var currItem in request.BidDetails ?? [])
            {
                if (leadingItemMap.TryGetValue(currItem.AuctionRequirementId, out var leadingItem))
                {
                    var itemName = requirementNameMap.TryGetValue(currItem.AuctionRequirementId, out var name) && !string.IsNullOrWhiteSpace(name)
                        ? name
                        : $"Item #{currItem.AuctionRequirementId}";

                    if (auction.IsForwardAuction && currItem.NetAmount <= leadingItem.NetAmount)
                    {
                        validationErrors.Add(new ValidationException(
                            $"Bid amount for item '{itemName}' must be greater than the leading item amount of {leadingItem.NetAmount:0.00}."));
                    }
                    else if (!auction.IsForwardAuction && currItem.NetAmount >= leadingItem.NetAmount)
                    {
                        validationErrors.Add(new ValidationException(
                            $"Bid amount for item '{itemName}' must be less than the leading item amount of {leadingItem.NetAmount:0.00}."));
                    }
                }
            }
        }

        return validationErrors;
    }
}
