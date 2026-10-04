using Core.Entities.TenantRelated;
using Core.Models.Tenant;

namespace Core.Abstraction.TenantRelated
{
    public interface ITenantRepository
    {
        Task<TenantSaveResponseDataModel> Register(Tenant input);
        Task<bool> LinkTenantToUser(int referenceId, int tenantId);
        Task<TenantProfileDataModel?> GetProfileAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<bool> IsTenantNameTakenAsync(string name, int excludeTenantId, CancellationToken cancellationToken = default);
        Task<TenantProfileDataModel> UpdateProfileAsync(int tenantId, string name, string about, int? foregroundImageId, CancellationToken cancellationToken = default);
        Task<OrganizationPublicProfileDataModel?> GetOrganizationPublicProfileAsync(int organizationId, CancellationToken cancellationToken = default);
    }
}
