using Core.Entities.TenantRelated;
using Core.Models.Models;

namespace Core.Abstraction.TenantRelated
{
    public interface IVendorRepository
    {
        Task<VendorSaveResponseDataModel> SaveVendor(Vendor input);
    }
}
