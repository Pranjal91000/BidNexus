using Core.Abstraction.Auth;
using Core.Abstraction.TenantRelated;
using Infrastructure.Repository.Auth;
using Infrastructure.Repository.GlobalData;
using Infrastructure.Repository.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Core.Abstraction.GlobalData;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Extensions
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionStringName = "DefaultConnection")
        {
            var connectionString = configuration.GetConnectionString(connectionStringName)
                ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' was not found.");

            return services.AddInfrastructureServices(connectionString);
        }

        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            string connectionString)
        {
            // Database Context
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Repositories
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IGlobalDataRepository, GlobalDataRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IVendorRepository, VendorRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();

            return services;
        }
    }
}
