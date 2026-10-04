using Core.Abstraction.Auth;
using Core.Abstraction.TenantRelated;
using Infrastructure.Repository.Auth;
using Infrastructure.Repository.GlobalData;
using Infrastructure.Repository.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Core.Abstraction.GlobalData;
using Microsoft.Extensions.Configuration;
using Core.Abstraction.Master;
using Core.Abstraction.Utilities;
using Infrastructure.Repository.Master;
using Infrastructure.Repository.Utilities;
using Infrastructure.Extensions.AuctionRel;

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
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<ITaxMasterRepository, TaxMasterRepository>();
            services.AddScoped<Core.Abstraction.Utilities.IRatingRepository, Infrastructure.Repository.Utilities.RatingRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();

            services.AddAuctionInfrastructure();

            return services;
        }
    }
}
