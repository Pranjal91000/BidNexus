using Core.Abstraction.AuctionRelated;
using Core.Abstraction.Services;
using Core.Models.AuctionRelated;

namespace Core.Services;

public sealed class AuctionStatementCoreService(
    IAuctionStatementRepository repository) : IAuctionStatementCoreService
{
    private readonly IAuctionStatementRepository _repository = repository;

    public Task GenerateAsync(
        int auctionId,
        CancellationToken cancellationToken = default)
        => _repository.GenerateAsync(auctionId, cancellationToken);

    public Task<List<AuctionStatementDataModel>> GetAsync(
        int auctionId,
        int tenantId,
        CancellationToken cancellationToken = default)
        => _repository.GetAsync(auctionId, tenantId, cancellationToken);

    public Task<VendorAuctionResultDataModel?> GetVendorResultAsync(
        int auctionId,
        int vendorId,
        CancellationToken cancellationToken = default)
        => _repository.GetVendorResultAsync(auctionId, vendorId, cancellationToken);
}
