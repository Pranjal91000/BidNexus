using API.Abstraction.AuctionRel;
using API.Services.AuctionRel;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions.AuctionRel;

public static class AuctionApiServiceCollectionExtensions
{
    public static IServiceCollection AddAuctionApiServices(this IServiceCollection services)
    {
        services.AddScoped<IAuctionService, AuctionService>();
        return services;
    }
}
