using Core.Entities.Auction;
using Core.Models.AuctionRelated;

namespace Core.Abstraction.AuctionRelated
{
    public interface IAuctionRepository
    {
        Task<AuctionResponseModel> AddAsync(Auction input);
        Task<AuctionResponseModel> UpdateAsync(Auction input);
        Task<AuctionDataModel> GetById(int id);
        Task<List<AuctionGetDataModel>> Get(short categoryId, short pageNo, short pageSize);
        Task<bool> DeleteAsync(int id);

        Task<List<AuctionLifecycleDataModel>> GetAuctionsForLifecycleAsync(
            DateTimeOffset now,
            string scheduledStatusName,
            string activeStatusName,
            string closedStatusName);

        Task UpdateStatusAsync(int auctionId, string statusName);
    }
}
