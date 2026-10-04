using Core.Entities.Auction;
using Core.Exceptions;
using Core.Models.AuctionRelated;

namespace Core.Abstraction.Services
{
    public interface IBidCoreService
    {
        List<ValidationException> CustomValidation(Bid bid, BidProcessingContext context, AuctionDataModel? auction = null);
    }
}
