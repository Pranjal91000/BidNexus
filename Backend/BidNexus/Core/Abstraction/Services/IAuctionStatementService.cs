using Core.Models.AuctionRelated;

namespace Core.Abstraction.Services;

public interface IAuctionStatementService
{
    Task GenerateAsync(
        int auctionId,
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<List<AuctionStatementDataModel>> GetAsync(
        int auctionId,
        int tenantId,
        CancellationToken cancellationToken = default);
}
