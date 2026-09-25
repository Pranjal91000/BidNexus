using Core.Entities.TenantRelated;
using Core.Models.Models;

namespace Core.Abstraction.TenantRelated
{
    public interface IOrganizationRepository
    {
        Task<OrganizationSaveResponseDataModel> SaveOrganization(Organization input);
    }
}
