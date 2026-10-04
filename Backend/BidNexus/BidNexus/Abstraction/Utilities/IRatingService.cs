using Core.Models.Utilities;

namespace API.Abstraction.Utilities;

public interface IRatingService
{
    Task<RatingDataModel> SubmitRatingAsync(
        RatingCreateRequest request,
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

    Task<TenantReputationDataModel> GetMyReputationAsync(
        CancellationToken cancellationToken = default);
}
