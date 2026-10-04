using Core.Models.AuctionRelated;

namespace Core.Abstraction.Services;

public interface IAuctionStatementCoreService
{
    Task GenerateAsync(
        int auctionId,
        CancellationToken cancellationToken = default);

    Task<List<AuctionStatementDataModel>> GetAsync(
        int auctionId,
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<VendorAuctionResultDataModel?> GetVendorResultAsync(
        int auctionId,
        int vendorId,
        CancellationToken cancellationToken = default);
}
