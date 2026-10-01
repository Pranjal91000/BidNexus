using Core.Models.AuctionRelated;

namespace Core.Abstraction.AuctionRelated;

public interface IAuctionStatementRepository
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
