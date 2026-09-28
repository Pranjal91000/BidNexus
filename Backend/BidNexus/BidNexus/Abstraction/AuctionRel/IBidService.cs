using API.Models.AuctionRel;
using Core.Models.AuctionRelated;

namespace API.Abstraction.AuctionRel;

public interface IBidService
{
    Task<BidResponseDataModel> ProcessBidAsync(BidCreateRequest request, CancellationToken cancellationToken = default);
    Task<List<BidDataModel>> GetAuctionBidsAsync(int auctionId, CancellationToken cancellationToken = default);
    Task<List<BidDataModel>> GetBidHistoryAsync(int vendorId, int auctionId, CancellationToken cancellationToken = default);
    Task<List<BidDataModel>> GetLeaderBoardAsync(int auctionId, CancellationToken cancellationToken = default);
    Task<BidDataModel> GetByIdAsync(long bidId, CancellationToken cancellationToken = default);
}
