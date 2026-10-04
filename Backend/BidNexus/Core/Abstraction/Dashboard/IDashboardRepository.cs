using Core.Models.Dashboard;

namespace Core.Abstraction.Dashboard;

public interface IDashboardRepository
{
    Task<OrganizationDashboardDataModel> GetOrganizationDashboardAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<VendorDashboardDataModel> GetVendorDashboardAsync(int vendorId, CancellationToken cancellationToken = default);
}
