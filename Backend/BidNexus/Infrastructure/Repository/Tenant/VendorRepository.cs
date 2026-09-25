using Core.Abstraction.TenantRelated;
using Core.Entities.TenantRelated;
using Core.Models.Models;

namespace Infrastructure.Repository.Tenant
{
    public class VendorRepository(AppDbContext dbContext) : IVendorRepository
    {
        private readonly AppDbContext appDbContext = dbContext;

        public async Task<VendorSaveResponseDataModel> SaveVendor(Vendor input)
        {
            await appDbContext.Vendors.AddAsync(input);
            var isSuccess = await appDbContext.SaveChangesAsync() > 0;

            if (!isSuccess) throw new InvalidOperationException("Failed to save Vendor.");

            return new VendorSaveResponseDataModel { Id = input.Id, Name = input.Name };
        }
    }
}
