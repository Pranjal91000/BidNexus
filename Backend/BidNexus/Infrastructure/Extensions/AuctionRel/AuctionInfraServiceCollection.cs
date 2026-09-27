using Core.Abstraction.AuctionRelated;
using Infrastructure.Repository.AuctionRel;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions.AuctionRel
{
    public static class AuctionInfraServiceCollection
    {
        public static IServiceCollection AddAuctionInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            return services;
        }
    }
}
