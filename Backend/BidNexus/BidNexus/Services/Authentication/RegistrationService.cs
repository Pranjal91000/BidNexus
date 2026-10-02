using API.Abstraction.Authentication;
using API.Models.Authentication;
using Core.Abstraction.Services;
using Core.Abstraction.TenantRelated;
using Core.Entities.TenantRelated;
using Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace API.Services.Authentication
{
    public class RegistrationService(
        ITenantRepository tenantRepository,
        IVendorRepository vendorRepository,
        IOrganizationRepository organizationRepository,
        IAuthenticationCoreService authenticationCoreService,
        IPasswordHasher<Tenant> passwordHasher,
        AppDbContext dbContext) : IRegistrationService
    {
        private readonly ITenantRepository _tenantRepository = tenantRepository;
        private readonly IVendorRepository _vendorRepository = vendorRepository;
        private readonly IOrganizationRepository _organizationRepository = organizationRepository;
        private readonly IAuthenticationCoreService _authCoreService = authenticationCoreService;
        private readonly IPasswordHasher<Tenant> _passwordHasher = passwordHasher;
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<RegistrationOutputModel> Register(RegistrationInputModel input)
        {
            if (string.IsNullOrWhiteSpace(input.Password))
                throw new ArgumentException("Password is required.", nameof(input.Password));

            var tenant = new Tenant(
                input.Name,
                input.ContactNumber,
                input.EmailAddress,
                input.UserName,
                string.Empty,
                !input.RegisterAsOrganization);

            tenant.PasswordHash = _passwordHasher.HashPassword(tenant, input.Password);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var tenantResponse = await _tenantRepository.Register(tenant);

                if (input.RegisterAsOrganization)
                {
                    var organization = new Organization(
                        tenantResponse.Id,
                        input.Name,
                        input.OfficialAddress,
                        input.About);

                    var organizationResponse =
                        await _organizationRepository.SaveOrganization(organization);

                    await _tenantRepository.LinkTenantToUser(
                        organizationResponse.Id,
                        tenantResponse.Id);
                }
                else
                {
                    var vendor = new Vendor(
                        tenantResponse.Id,
                        input.Name,
                        input.About);

                    var vendorResponse = await _vendorRepository.SaveVendor(vendor);

                    await _tenantRepository.LinkTenantToUser(
                        vendorResponse.Id,
                        tenantResponse.Id);
                }

                // Ensure changes persisted if repositories don't already call SaveChanges
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            var authToken = _authCoreService.GenerateAuthToken(
                tenant.ReferenceId,
                tenant.Id,
                input.EmailAddress,
                input.RegisterAsOrganization ? "Organization" : "Vendor");

            return new RegistrationOutputModel { AuthToken = authToken };
        }
    }
}
