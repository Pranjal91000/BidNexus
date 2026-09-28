using API.Abstraction.AuctionRel;
using API.Models.AuctionRel;
using API.ModelValidators.AuctionRel;
using API.Services.AuctionRel;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions.AuctionRel;

public static class AuctionApiServiceCollectionExtensions
{
    public static IServiceCollection AddAuctionApiServices(this IServiceCollection services)
    {
        services.AddScoped<IAuctionService, AuctionService>();
        services.AddScoped<IBidService, BidService>();
        services.AddScoped<IValidator<BidCreateRequest>, BidCreateRequestValidator>();
        return services;
    }
}
