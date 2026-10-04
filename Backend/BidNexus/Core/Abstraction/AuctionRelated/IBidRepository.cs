using Core.Entities.Auction;
using Core.Models.AuctionRelated;

namespace Core.Abstraction.AuctionRelated
{
    public interface IBidRepository
    {
        Task<BidProcessingContext> GetBidProcessingContextAsync(
            int auctionId,
            int vendorId,
            IReadOnlyCollection<int> requirementIds,
            IReadOnlyCollection<int> taxIds,
            IReadOnlyCollection<short> taxNatureIds,
            IReadOnlyCollection<short> chargeTypeIds,
            int tenantId,
            bool isForwardAuction,
            CancellationToken cancellationToken = default);

        Task<BidResponseDataModel> ProcessBidAsync(Bid currbid, long? invalidateBidId, CancellationToken cancellationToken = default);

        Task<List<BidDataModel>> GetAuctionBidsAsync(int auctionId);
        Task<List<BidDataModel>> GetBidHistory(int vendorId, int auctionId);
        Task<List<BidDataModel>> GetLeaderBoard(int auctionId);
        Task<BidDataModel> GetById(long bidId);
        Task<BidDataModel?> GetVendorsCurrentBidAsync(int vendorId, int auctionId);
        Task<BidDataModel?> GetLeadingBidForAuctionAsync(int auctionId, bool isForwardAuction);
    }
}