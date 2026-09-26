using API.Abstraction.GlobalData;
using Core.Entities.GlobalData;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.GlobalData;

[ApiController]
[Route("api/global-data")]
public class GlobalDataController(IGlobalDataService service) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories(CancellationToken cancellationToken) =>
        Ok(await service.GetCategories(cancellationToken));

    [HttpGet("charge-types")]
    public async Task<ActionResult<IEnumerable<ChargeType>>> GetChargeTypes(CancellationToken cancellationToken) =>
        Ok(await service.GetChargeTypes(cancellationToken));

    [HttpGet("rating-fors")]
    public async Task<ActionResult<IEnumerable<RatingFor>>> GetRatingFors(CancellationToken cancellationToken) =>
        Ok(await service.GetRatingFors(cancellationToken));

    [HttpGet("rating-parameters")]
    public async Task<ActionResult<IEnumerable<RatingParameter>>> GetRatingParameters(CancellationToken cancellationToken) =>
        Ok(await service.GetRatingParameters(cancellationToken));

    [HttpGet("statuses")]
    public async Task<ActionResult<IEnumerable<Status>>> GetStatuses(CancellationToken cancellationToken) =>
        Ok(await service.GetStatuses(cancellationToken));

    [HttpGet("tax-natures")]
    public async Task<ActionResult<IEnumerable<TaxNature>>> GetTaxNatures(CancellationToken cancellationToken) =>
        Ok(await service.GetTaxNatures(cancellationToken));
}
