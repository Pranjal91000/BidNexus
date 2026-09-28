using API.Abstraction.AuctionRel;
using API.Models.AuctionRel;
using API.ModelValidators.AuctionRel;
using Core.Abstraction.AuctionRelated;
using Core.Entities.Auction;
using Core.Models.AuctionRelated;

namespace API.Services.AuctionRel;

public class BidService(IBidRepositoy bidRepository) : IBidService
{
    private readonly IBidRepositoy _bidRepository = bidRepository;

    public async Task<BidResponseDataModel> ProcessBidAsync(BidCreateRequest request, CancellationToken cancellationToken = default)
    {
        var bid = new Bid
        {
            AuctionId = request.AuctionId,
            VendorId = request.VendorId,
            BasicAmount = request.BasicAmount,
            TaxAmount = request.TaxAmount,
            DiscountAmount = request.DiscountAmount,
            NetAmount = request.NetAmount,
            CreatedAt = DateTimeOffset.UtcNow,
            IsCurrent = true,
            MainBidId = request.MainBidId ?? 0,
            BidRevisionNo = (short) (request.BidRevisionNo > 0 ? request.BidRevisionNo : (short)1),
            BidDetails = (request.BidDetails ?? []).Select(bd => new BidDetail
            {
                AuctionRequirementId = bd.AuctionRequirementId,
                Rate = bd.Rate,
                BaseAmount = bd.BaseAmount,
                NetAmount = bd.NetAmount,
                Taxes = (bd.Taxes ?? []).Select(t => new BidTaxDetail
                {
                    TaxId = t.TaxId,
                    TaxName = t.TaxName?.Trim() ?? string.Empty,
                    TaxCode = t.TaxCode?.Trim() ?? string.Empty,
                    TaxNatureId = t.TaxNatureId,
                    ChargeTypeId = t.ChargeTypeId,
                    TaxValue = t.TaxValue,
                    TaxAmount = t.TaxAmount
                }).ToList()
            }).ToList()
        };

        return await _bidRepository.ProcessBidAsync(bid);
    }

    public async Task<List<BidDataModel>> GetAuctionBidsAsync(int auctionId, CancellationToken cancellationToken = default)
    {
        return await _bidRepository.GetAuctionBidsAsync(auctionId);
    }

    public async Task<List<BidDataModel>> GetBidHistoryAsync(int vendorId, int auctionId, CancellationToken cancellationToken = default)
    {
        return await _bidRepository.GetBidHistory(vendorId, auctionId);
    }

    public async Task<List<BidDataModel>> GetLeaderBoardAsync(int auctionId, CancellationToken cancellationToken = default)
    {
        return await _bidRepository.GetLeaderBoard(auctionId);
    }

    public async Task<BidDataModel> GetByIdAsync(long bidId, CancellationToken cancellationToken = default)
    {
        return await _bidRepository.GetById(bidId);
    }
}
