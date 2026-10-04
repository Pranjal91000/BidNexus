using API.Abstraction.Master;
using API.Models.Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Master;

[ApiController]
[Authorize]
[Route("api/masters/items")]
public class ItemController(IItemService itemService) : ControllerBase
{
    private readonly IItemService _itemService = itemService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ItemResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _itemService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _itemService.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound(new { message = $"Item with ID {id} not found." });
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Organization,Admin")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ItemResponse>> Create([FromBody] ItemCreateRequest request, CancellationToken cancellationToken)
    {
        var validation = request.Validate();
        if (!validation.IsValid)
        {
            return BadRequest(validation.ToDictionary());
        }

        var created = await _itemService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organization,Admin")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ItemResponse>> Update(int id, [FromBody] ItemUpdateRequest request, CancellationToken cancellationToken)
    {
        var validation = request.Validate();
        if (!validation.IsValid)
        {
            return BadRequest(validation.ToDictionary());
        }

        var updated = await _itemService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organization,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _itemService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
