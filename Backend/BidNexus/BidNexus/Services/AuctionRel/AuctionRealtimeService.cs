using Core.Abstraction.Services;
using Microsoft.AspNetCore.SignalR;

namespace API.Services.AuctionRel;

public sealed class AuctionRealtimeService(
    IHubContext<AuctionHub> hubContext) : IAuctionRealtimeService
{
    private readonly IHubContext<AuctionHub> _hubContext = hubContext;

    public Task PublishBidAcceptedAsync(
        int auctionId,
        long bidId,
        int vendorId,
        decimal netAmount,
        CancellationToken cancellationToken = default)
        => _hubContext.Clients
            .Group(AuctionHub.GetGroupName(auctionId))
            .SendAsync(
                "BidAccepted",
                new
                {
                    AuctionId = auctionId,
                    BidId = bidId,
                    VendorId = vendorId,
                    NetAmount = netAmount
                },
                cancellationToken);

    public Task PublishAuctionClosedAsync(
        int auctionId,
        CancellationToken cancellationToken = default)
        => _hubContext.Clients
            .Group(AuctionHub.GetGroupName(auctionId))
            .SendAsync(
                "AuctionClosed",
                new { AuctionId = auctionId },
                cancellationToken);
}
