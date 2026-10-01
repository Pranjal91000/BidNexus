using API.Models.AuctionRel;
using Core.Models.AuctionRelated;

namespace API.Abstraction.AuctionRel;

public interface IAuctionEngine
{
    Task<BidResponseDataModel> ProcessBidAsync(
        BidCreateRequest request,
        CancellationToken cancellationToken = default);
}
