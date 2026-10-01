using Core.Abstraction.AuctionRelated;
using Core.Abstraction.Services;
using Core.Models.AuctionRelated;

namespace Core.Services;

public sealed class AuctionStatementService(
    IAuctionStatementRepository repository) : IAuctionStatementService
{
    private readonly IAuctionStatementRepository _repository = repository;

    public Task GenerateAsync(
        int auctionId,
        int tenantId,
        CancellationToken cancellationToken = default)
        => _repository.GenerateAsync(auctionId, tenantId, cancellationToken);

    public Task<List<AuctionStatementDataModel>> GetAsync(
        int auctionId,
        int tenantId,
        CancellationToken cancellationToken = default)
        => _repository.GetAsync(auctionId, tenantId, cancellationToken);
}
