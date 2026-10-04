using Core.Entities.Utilities;
using Core.Models.Utilities;

namespace Core.Abstraction.Utilities;

public interface IRatingRepository
{
    Task<RatingDataModel> SubmitRatingAsync(
        Rating rating,
        CancellationToken cancellationToken = default);

    Task<TenantReputationDataModel> GetTenantReputationAsync(
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<List<RatingDataModel>> GetRatingsAgainstTenantAsync(
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<List<RatingDataModel>> GetRatingsForAuctionAsync(
        int auctionId,
        CancellationToken cancellationToken = default);

    Task<bool> HasRatingBeenSubmittedAsync(
        int auctionId,
        short ratingForId,
        int submittedByTenant,
        CancellationToken cancellationToken = default);
}
