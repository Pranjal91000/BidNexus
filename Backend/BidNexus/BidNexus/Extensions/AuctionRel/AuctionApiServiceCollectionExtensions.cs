using API.Abstraction.AuctionRel;
using API.Models.AuctionRel;
using API.ModelValidators.AuctionRel;
using API.Services.AuctionRel;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace API.Extensions.AuctionRel;

public static class AuctionApiServiceCollectionExtensions
{
    public static IServiceCollection AddAuctionApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuctionService, AuctionService>();
        services.AddScoped<IBidService, BidService>();
        services.AddSingleton<IAuctionEngine, AuctionEngine>();
        services.Configure<AuctionEngineOptions>(
            configuration.GetSection(AuctionEngineOptions.SectionName));
        services.AddHostedService<AuctionLifecycleWorker>();
        services.AddScoped<IValidator<BidCreateRequest>, BidCreateRequestValidator>();
        return services;
    }
}
