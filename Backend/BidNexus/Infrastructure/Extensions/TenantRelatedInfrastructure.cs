using Core.Abstraction.TenantRelated;
using Infrastructure.Repository.Authentication;
using Infrastructure.Repository.Tenant;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class TenantRelatedInfrastructure
    {
        public static IServiceCollection AddTenantRelatedInfrastructure(
            this IServiceCollection services)
        {
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IVendorRepository, VendorRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();

            return services;
        }
    }
}
