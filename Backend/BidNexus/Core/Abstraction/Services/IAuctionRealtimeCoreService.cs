namespace Core.Abstraction.Services;

public interface IAuctionRealtimeCoreService
{
    Task PublishBidAcceptedAsync(
        int auctionId,
        long bidId,
        int vendorId,
        decimal netAmount,
        CancellationToken cancellationToken = default);

    Task PublishAuctionStartedAsync(
        int auctionId,
        CancellationToken cancellationToken = default);

    Task PublishAuctionCompletedAsync(
        int auctionId,
        CancellationToken cancellationToken = default);

    Task PublishAuctionClosedAsync(
        int auctionId,
        CancellationToken cancellationToken = default);
}
