using API.Abstraction.Utilities;
using Core.Models.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Utilities;

[ApiController]
[Authorize]
[Route("api/ratings")]
public class RatingController(IRatingService ratingService) : ControllerBase
{
    private readonly IRatingService _ratingService = ratingService;

    [HttpPost]
    [ProducesResponseType(typeof(RatingDataModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RatingDataModel>> SubmitRating(
        [FromBody] RatingCreateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ratingService.SubmitRatingAsync(request, cancellationToken);
            return CreatedAtAction(
                nameof(GetRatingsForAuction),
                new { auctionId = result.AuctionId },
                result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("tenant/{tenantId:int}/reputation")]
    [ProducesResponseType(typeof(TenantReputationDataModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TenantReputationDataModel>> GetTenantReputation(
        int tenantId,
        CancellationToken cancellationToken)
    {
        var result = await _ratingService.GetTenantReputationAsync(tenantId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("tenant/{tenantId:int}/history")]
    [ProducesResponseType(typeof(List<RatingDataModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<RatingDataModel>>> GetTenantRatingHistory(
        int tenantId,
        CancellationToken cancellationToken)
    {
        var result = await _ratingService.GetRatingsAgainstTenantAsync(tenantId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("auction/{auctionId:int}")]
    [ProducesResponseType(typeof(List<RatingDataModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<RatingDataModel>>> GetRatingsForAuction(
        int auctionId,
        CancellationToken cancellationToken)
    {
        var result = await _ratingService.GetRatingsForAuctionAsync(auctionId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("my-reputation")]
    [ProducesResponseType(typeof(TenantReputationDataModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<TenantReputationDataModel>> GetMyReputation(
        CancellationToken cancellationToken)
    {
        var result = await _ratingService.GetMyReputationAsync(cancellationToken);
        return Ok(result);
    }
}
