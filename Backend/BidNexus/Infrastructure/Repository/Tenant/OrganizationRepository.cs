using Core.Abstraction.TenantRelated;
using Core.Entities.TenantRelated;
using Core.Models.Models;

namespace Infrastructure.Repository.Tenant
{
    public class OrganizationRepository(AppDbContext dbContext) : IOrganizationRepository
    {
        private readonly AppDbContext appDbContext = dbContext;

        public async Task<OrganizationSaveResponseDataModel> SaveOrganization(Organization input)
        {
            await appDbContext.Organizations.AddAsync(input);
            var isSuccess = await appDbContext.SaveChangesAsync() > 0;

            if (!isSuccess) throw new InvalidOperationException("Failed to save Organization.");

            return new OrganizationSaveResponseDataModel { Id = input.Id, Name = input.Name }; 
        }
    }
}
