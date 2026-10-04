using API.Abstraction.Authentication;
using API.Abstraction.GlobalData;
using API.Extensions.AuctionRel;
using API.Extensions.Master;
using API.Services.Authentication;
using API.Services.GlobalData;

namespace API.Extensions
{
    public static class ApiServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IGlobalDataService, GlobalDataService>();

            // Master Services
            services.AddMasterApiServices();

            // Auction Services
            services.AddAuctionApiServices(configuration);

            // Attachment & Profile Services
            services.AddScoped<API.Abstraction.Utilities.IAttachmentService, API.Services.Utilities.AttachmentService>();
            services.AddScoped<API.Abstraction.Profile.IProfileService, API.Services.Profile.ProfileService>();

            return services;
        }
    }
}
