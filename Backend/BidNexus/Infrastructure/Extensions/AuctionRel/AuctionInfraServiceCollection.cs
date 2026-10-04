using Core.Abstraction.AuctionRelated;
using Core.Abstraction.Dashboard;
using Infrastructure.Repository.AuctionRel;
using Infrastructure.Repository.Dashboard;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions.AuctionRel
{
    public static class AuctionInfraServiceCollection
    {
        public static IServiceCollection AddAuctionInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            services.AddScoped<IBidRepository, BiddingRepository>();
            services.AddScoped<IAuctionStatementRepository, AuctionStatementRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            return services;
        }
    }
}