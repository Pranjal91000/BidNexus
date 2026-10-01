using API.Abstraction.AuctionRel;
using API.Models.AuctionRel;
using Core.Models.AuctionRelated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.AuctionRel;

[ApiController]
[Authorize]
[Route("api/bids")]
public class BidController(IAuctionEngine auctionEngine) : ControllerBase
{
    private readonly IAuctionEngine _auctionEngine = auctionEngine;
    
    [HttpPost]
    [ProducesResponseType(typeof(BidResponseDataModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BidResponseDataModel>> ProcessBid([FromBody] BidCreateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _auctionEngine.ProcessBidAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(ex.Errors.ToDictionary(
                error => error.PropertyName,
                error => new[] { error.ErrorMessage }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(BidDataModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BidDataModel>> GetById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _bidService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("auction/{auctionId:int}")]
    [ProducesResponseType(typeof(List<BidDataModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BidDataModel>>> GetAuctionBids(int auctionId, CancellationToken cancellationToken)
    {
        var result = await _bidService.GetAuctionBidsAsync(auctionId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("auction/{auctionId:int}/leaderboard")]
    [ProducesResponseType(typeof(List<BidDataModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BidDataModel>>> GetLeaderBoard(int auctionId, CancellationToken cancellationToken)
    {
        var result = await _bidService.GetLeaderBoardAsync(auctionId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("auction/{auctionId:int}/history")]
    [ProducesResponseType(typeof(List<BidDataModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BidDataModel>>> GetBidHistory(
        int auctionId,
        [FromQuery] int vendorId,
        CancellationToken cancellationToken)
    {
        var result = await _bidService.GetBidHistoryAsync(vendorId, auctionId, cancellationToken);
        return Ok(result);
    }
}
