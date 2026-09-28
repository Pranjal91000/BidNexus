using Core.Entities.Auction;
using Core.Models.AuctionRelated;

namespace Core.Abstraction.AuctionRelated
{
    public interface IBidRepositoy
    {
        Task<BidResponseDataModel> ProcessBidAsync(Bid bid);
        Task<List<BidDataModel>> GetAuctionBidsAsync(int auctionId);
        Task<List<BidDataModel>> GetBidHistory(int vendorId, int auctionId);
        Task<List<BidDataModel>> GetLeaderBoard(int auctionId);
        Task<BidDataModel> GetById(long bidId);
        Task<BidDataModel?> GetVendorsCurrentBidAsync(int vendorId, int auctionId);
    }
}
