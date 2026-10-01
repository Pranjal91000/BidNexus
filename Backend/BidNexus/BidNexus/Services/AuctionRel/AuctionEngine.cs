using System.Collections.Concurrent;
using API.Abstraction.AuctionRel;
using API.Models.AuctionRel;
using Core.Abstraction.AuctionRelated;
using FluentValidation;

namespace API.Services.AuctionRel;

public sealed class AuctionEngine(
    IAuctionRepository auctionRepository,
    IBidService bidService,
    IValidator<BidCreateRequest> validator,
    Microsoft.Extensions.Options.IOptions<AuctionEngineOptions> options) : IAuctionEngine
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IBidService _bidService = bidService;
    private readonly IValidator<BidCreateRequest> _validator = validator;
    private readonly AuctionEngineOptions _options = options.Value;

    // One serializer per auction. Different auctions can process concurrently,
    // while bids for the same auction are processed sequentially.
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _auctionLocks = new();

    public async Task<BidResponseDataModel> ProcessBidAsync(
        BidCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.AuctionId <= 0)
            throw new ArgumentException("AuctionId must be greater than zero.", nameof(request));

        var auctionLock = _auctionLocks.GetOrAdd(
            request.AuctionId,
            static _ => new SemaphoreSlim(1, 1));

        await auctionLock.WaitAsync(cancellationToken);

        try
        {
            var auction = await _auctionRepository.GetById(request.AuctionId);
            var now = DateTimeOffset.UtcNow;

            if (now < auction.AuctionStartTime)
                throw new InvalidOperationException("Auction has not started yet.");

            if (now >= auction.AuctionEndTime)
                throw new InvalidOperationException("Auction has already ended.");

            if (!string.Equals(
                    auction.StatusName,
                    _options.ActiveStatusName,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Auction is not active. Current status: {auction.StatusName ?? "Unknown"}.");
            }

            var validation = await _validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            return await _bidService.ProcessBidAsync(request, cancellationToken);
        }
        finally
        {
            auctionLock.Release();
        }
    }
}
