using Core.Abstraction.Services;
using Core.Abstraction.TenantRelated;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{
    public static class CoreServiceCollection
    {
        public static IServiceCollection CoreServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IJwtHelperService, JwtHelperService>();
            services.AddScoped<IAuthenticationCoreService, AuthenticationCoreService>();
            return services;
        }
    }
}
