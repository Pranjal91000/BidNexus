using Core.Abstraction.Services;
using Core.Services;
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
            return services;
        }
    }
}
