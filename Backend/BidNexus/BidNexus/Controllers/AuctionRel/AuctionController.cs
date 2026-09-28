using API.Abstraction.AuctionRel;
using API.Models.AuctionRel;
using Core.Models.AuctionRelated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.AuctionRel;

[ApiController]
[Authorize]
[Route("api/auctions")]
public class AuctionController(IAuctionService auctionService) : ControllerBase
{
    private readonly IAuctionService _auctionService = auctionService;

    [HttpGet]
    [ProducesResponseType(typeof(List<AuctionGetDataModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AuctionGetDataModel>>> Get(
        [FromQuery] short categoryId = 0,
        [FromQuery] short pageNo = 1,
        [FromQuery] short pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _auctionService.GetAsync(categoryId, pageNo, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AuctionDataModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuctionDataModel>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _auctionService.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound(new { message = $"Auction with ID {id} not found." });
        }

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AuctionResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuctionResponseModel>> Create([FromBody] AuctionCreateRequest request, CancellationToken cancellationToken)
    {
        var validation = request.Validate();
        if (!validation.IsValid)
        {
            return BadRequest(validation.ToDictionary());
        }

        var created = await _auctionService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AuctionResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuctionResponseModel>> Update([FromRoute] int id, [FromBody] AuctionUpdateRequest request, CancellationToken cancellationToken)
    {
        var validation = request.Validate();
        if (!validation.IsValid)
        {
            return BadRequest(validation.ToDictionary());
        }

        try
        {
            var updated = await _auctionService.UpdateAsync(id, request, cancellationToken);
            return Ok(updated);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Not Found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _auctionService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
