using API.Abstraction.Authentication;
using API.Abstraction.GlobalData;
using API.Extensions.Master;
using API.Services.Authentication;
using API.Services.GlobalData;

namespace API.Extensions
{
    public static class ApiServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IGlobalDataService, GlobalDataService>();

            // Master Services
            services.AddMasterApiServices();

            return services;
        }
    }
}
