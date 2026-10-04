using Core.Abstraction.Services;
using Core.Services;
using Core.Entities.TenantRelated;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{
    public static class CoreServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IJwtHelperService, JwtHelperService>();
            services.AddScoped<IAuthenticationCoreService, AuthenticationCoreService>();
            services.AddScoped<IBidCoreService, BidCoreService>();
            services.AddScoped<IAuctionEngine, AuctionEngine>();
            services.AddScoped<IAuctionStatementCoreService, AuctionStatementCoreService>();
            services.AddScoped<IPasswordHasher<Tenant>, PasswordHasher<Tenant>>();
            return services;
        }
    }
}