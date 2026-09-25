using API.Abstraction.Authentication;
using API.Services.Authentication;

namespace API.Extensions
{
    public static class APIServices
    {
        public static IServiceCollection APIServiceCollection(this IServiceCollection services)
        {
            services.AddScoped<IRegistrationService, RegistrationService>();
            return services;
        }

    }
}
