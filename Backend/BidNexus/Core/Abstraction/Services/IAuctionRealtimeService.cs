namespace Core.Abstraction.Services;

public interface IAuctionRealtimeService
{
    Task PublishBidAcceptedAsync(
        int auctionId,
        long bidId,
        int vendorId,
        decimal netAmount,
        CancellationToken cancellationToken = default);

    Task PublishAuctionClosedAsync(
        int auctionId,
        CancellationToken cancellationToken = default);
}
