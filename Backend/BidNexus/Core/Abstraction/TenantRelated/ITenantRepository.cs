using Core.Entities.TenantRelated;
using Core.Models.Tenant;

namespace Core.Abstraction.TenantRelated
{
    public interface ITenantRepository
    {
        Task<TenantSaveResponseDataModel> Register(Tenant input);
        Task<bool> LinkTenantToUser(int referenceId, int tenantId);
    }
}
