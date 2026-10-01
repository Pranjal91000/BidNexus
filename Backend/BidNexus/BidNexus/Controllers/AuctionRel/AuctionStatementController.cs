using Core.Abstraction.Services;
using Core.Models.AuctionRelated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.AuctionRel;

[ApiController]
[Authorize(Roles = "Organization")]
[Route("api/auctions")]
public sealed class AuctionStatementController(
    IAuctionStatementService statementService,
    IJwtHelperService jwtHelperService) : ControllerBase
{
    private readonly IAuctionStatementService _statementService = statementService;
    private readonly IJwtHelperService _jwtHelperService = jwtHelperService;

    [HttpGet("{auctionId:int}/statement")]
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
}
