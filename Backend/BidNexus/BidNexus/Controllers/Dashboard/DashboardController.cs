using Core.Abstraction.Dashboard;
using Core.Abstraction.Services;
using Core.Models.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Dashboard;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController(
    IDashboardRepository dashboardRepository,
    IJwtHelperService jwtHelperService) : ControllerBase
{
    private readonly IDashboardRepository _dashboard = dashboardRepository;
    private readonly IJwtHelperService _jwt = jwtHelperService;

    [HttpGet("organization")]
    [Authorize(Roles = "Organization")]
    [ProducesResponseType(typeof(OrganizationDashboardDataModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrganizationDashboardDataModel>> GetOrganization(CancellationToken cancellationToken)
        => Ok(await _dashboard.GetOrganizationDashboardAsync(_jwt.GetTenantId(), cancellationToken));

    [HttpGet("vendor")]
    [Authorize(Roles = "Vendor")]
    [ProducesResponseType(typeof(VendorDashboardDataModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<VendorDashboardDataModel>> GetVendor(CancellationToken cancellationToken)
        => Ok(await _dashboard.GetVendorDashboardAsync(_jwt.GetUserId(), cancellationToken));
}
