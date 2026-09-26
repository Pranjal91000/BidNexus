using Core.Abstraction.TenantRelated;
using Core.Models.Tenant;
using Microsoft.EntityFrameworkCore;
using TenantEntity = Core.Entities.TenantRelated.Tenant;

namespace Infrastructure.Repository.Tenant
{
    public class TenantRepository(AppDbContext dbContext) : ITenantRepository
    {
        private readonly AppDbContext appDbContext = dbContext;

        public async Task<TenantSaveResponseDataModel> Register(TenantEntity input)
        {
            await appDbContext.Tenants.AddAsync(input);
            var isSuccess = await appDbContext.SaveChangesAsync() > 0;

            if (!isSuccess) throw new InvalidOperationException("Failed to save Tenant.");

            return new TenantSaveResponseDataModel
            {
                Id = input.Id,
                Name = input.Name
            };
        }

        public async Task<bool> LinkTenantToUser(int referenceId, int tenantId)
        {
            var tenant = await appDbContext.Tenants.FirstOrDefaultAsync(x => x.Id == tenantId);
            if (tenant == null) throw new KeyNotFoundException();
            tenant.AddReferenceId(referenceId);

            var success = await appDbContext.SaveChangesAsync() > 0;

            if (!success) throw new InvalidOperationException("Failed to add Reference Id for Tenant.");

            return success;
        }
    }
}
