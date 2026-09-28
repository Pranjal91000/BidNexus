using API.Abstraction.Master;
using API.Models.Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Master;

[ApiController]
[Authorize]
[Route("api/masters/units")]
public class UnitController(IUnitService unitService) : ControllerBase
{
    private readonly IUnitService _unitService = unitService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UnitResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UnitResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _unitService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UnitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnitResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _unitService.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound(new { message = $"Unit with ID {id} not found." });
        }

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UnitResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UnitResponse>> Create([FromBody] UnitCreateRequest request, CancellationToken cancellationToken)
    {
        var validation = request.Validate();
        if (!validation.IsValid)
        {
            return BadRequest(validation.ToDictionary());
        }

        var created = await _unitService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UnitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UnitResponse>> Update(int id, [FromBody] UnitUpdateRequest request, CancellationToken cancellationToken)
    {
        var validation = request.Validate();
        if (!validation.IsValid)
        {
            return BadRequest(validation.ToDictionary());
        }

        var updated = await _unitService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _unitService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
