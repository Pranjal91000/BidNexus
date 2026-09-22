using Core.Entities.GlobalData;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.GlobalData;

[ApiController]
[Route("api/global-data")]
public class GlobalDataController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories(CancellationToken cancellationToken)
    {
        var data = await dbContext.Categories
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpGet("charge-types")]
    public async Task<ActionResult<IEnumerable<ChargeType>>> GetChargeTypes(CancellationToken cancellationToken)
    {
        var data = await dbContext.ChargeTypes
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpGet("rating-fors")]
    public async Task<ActionResult<IEnumerable<RatingFor>>> GetRatingFors(CancellationToken cancellationToken)
    {
        var data = await dbContext.RatingFors
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpGet("rating-parameters")]
    public async Task<ActionResult<IEnumerable<RatingParameter>>> GetRatingParameters(CancellationToken cancellationToken)
    {
        var data = await dbContext.RatingParameters
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpGet("statuses")]
    public async Task<ActionResult<IEnumerable<Status>>> GetStatuses(CancellationToken cancellationToken)
    {
        var data = await dbContext.Statuses
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpGet("tax-natures")]
    public async Task<ActionResult<IEnumerable<TaxNature>>> GetTaxNatures(CancellationToken cancellationToken)
    {
        var data = await dbContext.TaxNatures
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return Ok(data);
    }
}
