using API.Abstraction.Master;
using API.Models.Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Master;

[ApiController]
[Authorize]
[Route("api/masters/tax-masters")]
public class TaxMasterController(ITaxMasterService taxMasterService) : ControllerBase
{
    private readonly ITaxMasterService _taxMasterService = taxMasterService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TaxMasterResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TaxMasterResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _taxMasterService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaxMasterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaxMasterResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _taxMasterService.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound(new { message = $"TaxMaster with ID {id} not found." });
        }

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaxMasterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaxMasterResponse>> Create([FromBody] TaxMasterCreateRequest request, CancellationToken cancellationToken)
    {
        var validation = request.Validate();
        if (!validation.IsValid)
        {
            return BadRequest(validation.ToDictionary());
        }

        var created = await _taxMasterService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TaxMasterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaxMasterResponse>> Update(int id, [FromBody] TaxMasterUpdateRequest request, CancellationToken cancellationToken)
    {
        var validation = request.Validate();
        if (!validation.IsValid)
        {
            return BadRequest(validation.ToDictionary());
        }

        var updated = await _taxMasterService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _taxMasterService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
