using Core.Abstraction.Services;
using Microsoft.AspNetCore.SignalR;

namespace Core.Services;

public sealed class AuctionRealtimeCoreService(
    IHubContext<AuctionHub> hubContext) : IAuctionRealtimeCoreService
{
    private readonly IHubContext<AuctionHub> _hubContext = hubContext;

    public Task PublishBidAcceptedAsync(int auctionId,long bidId,int vendorId,decimal netAmount,CancellationToken cancellationToken=default)
        => _hubContext.Clients.Group(AuctionHub.GetGroupName(auctionId))
            .SendAsync("BidAccepted", new { AuctionId=auctionId, BidId=bidId, VendorId=vendorId, NetAmount=netAmount }, cancellationToken);

    public Task PublishAuctionStartedAsync(int auctionId, CancellationToken cancellationToken = default)
        => _hubContext.Clients.Group(AuctionHub.GetGroupName(auctionId))
            .SendAsync("AuctionStarted", new { AuctionId = auctionId }, cancellationToken);

    public Task PublishAuctionCompletedAsync(int auctionId, CancellationToken cancellationToken = default)
        => _hubContext.Clients.Group(AuctionHub.GetGroupName(auctionId))
            .SendAsync("AuctionCompleted", new { AuctionId = auctionId }, cancellationToken);

    public Task PublishAuctionClosedAsync(int auctionId, CancellationToken cancellationToken = default)
        => PublishAuctionCompletedAsync(auctionId, cancellationToken);
}