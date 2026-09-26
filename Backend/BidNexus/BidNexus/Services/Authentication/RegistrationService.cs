using API.Abstraction.Authentication;
using API.Models.Authentication;
using Core.Abstraction.Services;
using Core.Abstraction.TenantRelated;
using Core.Entities.TenantRelated;
using Infrastructure;
using Infrastructure.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services.Authentication
{
    public class RegistrationService(
        ITenantRepository tenantRepository, 
        IVendorRepository vendorRepository, 
        IOrganizationRepository organizationRepository,
        IAuthenticationCoreService authenticationCoreService,
        AppDbContext dbContext): IRegistrationService
    {
        private readonly ITenantRepository _tenantRepository = tenantRepository;
        private readonly IVendorRepository _vendorRepository = vendorRepository;
        private readonly IOrganizationRepository _organizationRepository = organizationRepository;
        public readonly IAuthenticationCoreService _authCoreService = authenticationCoreService;
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<RegistrationOutputModel> Register(RegistrationInputModel input)
        {
            var tenant = new Tenant(input.Name, input.ContactNumber, input.EmailAddress, input.UserName, input.Password, false);


            await _dbContext.ExecuteTransactionalAsync(async () =>
            {
                var tenantresponse = await _tenantRepository.Register(tenant);
                if (input.RegisterAsOrganization)
                {
                    var organization = new Organization(tenantresponse.Id, input.Name, input.OfficialAddress, input.About);
                    var organizationRes = await _organizationRepository.SaveOrganization(organization);
                    await _tenantRepository.LinkTenantToUser(organizationRes.Id, tenant.Id);
                }
                else
                {
                    var vendor = new Vendor(tenantresponse.Id, input.Name, input.About);
                    var vendorRes = await _vendorRepository.SaveVendor(vendor);
                    await _tenantRepository.LinkTenantToUser(vendorRes.Id, tenant.Id);
                }

                
            });

            var authToken = _authCoreService.GenerateAuthToken(tenant.ReferenceId, tenant.Id, input.EmailAddress, input.RegisterAsOrganization ? "Organization" : "Vendor");

            return new RegistrationOutputModel { AuthToken = authToken}; 

        }


    
    }
}
