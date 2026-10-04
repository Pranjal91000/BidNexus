using Core.Entities.Auction;
using Core.Models.AuctionRelated;

namespace Core.Abstraction.Services;

public interface IAuctionEngine
{
    Task<BidResponseDataModel> ProcessBidAsync(
        Bid request,
        CancellationToken cancellationToken = default);
}
