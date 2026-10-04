using Core.Abstraction.Services;
using Core.Models.AuctionRelated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.AuctionRel;

[ApiController]
[Authorize]
[Route("api/auctions")]
public sealed class AuctionStatementController(
    IAuctionStatementCoreService statementService,
    IJwtHelperService jwtHelperService) : ControllerBase
{
    private readonly IAuctionStatementCoreService _statementService = statementService;
    private readonly IJwtHelperService _jwtHelperService = jwtHelperService;

    /// <summary>Full ranked statement. Only the organisation that owns the auction can read it.</summary>
    [HttpGet("{auctionId:int}/statement")]
    [Authorize(Roles = "Organization")]
    [ProducesResponseType(typeof(List<AuctionStatementDataModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AuctionStatementDataModel>>> GetStatement(
        int auctionId,
        CancellationToken cancellationToken)
    {
        var tenantId = _jwtHelperService.GetTenantId();

        var statement = await _statementService.GetAsync(
            auctionId,
            tenantId,
            cancellationToken);

        if (statement.Count == 0)
            return NotFound(new { message = "Auction statement was not found." });

        return Ok(statement);
    }

    /// <summary>The calling vendor's own outcome: rank, own bid, and the winning amount when prices are visible.</summary>
    [HttpGet("{auctionId:int}/my-result")]
    [Authorize(Roles = "Vendor")]
    [ProducesResponseType(typeof(VendorAuctionResultDataModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorAuctionResultDataModel>> GetMyResult(
        int auctionId,
        CancellationToken cancellationToken)
    {
        var vendorId = _jwtHelperService.GetUserId();
        var result = await _statementService.GetVendorResultAsync(auctionId, vendorId, cancellationToken);

        if (result == null)
            return NotFound(new { message = "Auction was not found." });

        return Ok(result);
    }
}
