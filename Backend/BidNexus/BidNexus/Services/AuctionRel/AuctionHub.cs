using Core.Abstraction.AuctionRelated;
using Core.Abstraction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Services.AuctionRel;

[Authorize]
public sealed class AuctionHub(
    IAuctionRepository auctionRepository,
    IJwtHelperService jwtHelperService) : Hub
{
    public const string Route = "/hubs/auction";

    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IJwtHelperService _jwtHelperService = jwtHelperService;

    public async Task JoinAuction(int auctionId)
    {
        if (auctionId <= 0)
            throw new HubException("AuctionId must be greater than zero.");

        var allowed = await _auctionRepository.CanAccessAuctionAsync(
            auctionId,
            _jwtHelperService.GetUserId(),
            _jwtHelperService.GetTenantId(),
            _jwtHelperService.GetRole(),
            Context.ConnectionAborted);

        if (!allowed)
            throw new HubException("You are not allowed to join this auction.");

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            GetGroupName(auctionId),
            Context.ConnectionAborted);
    }

    public Task LeaveAuction(int auctionId)
        => Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            GetGroupName(auctionId),
            Context.ConnectionAborted);

    public static string GetGroupName(int auctionId)
        => $"auction:{auctionId}";
}
