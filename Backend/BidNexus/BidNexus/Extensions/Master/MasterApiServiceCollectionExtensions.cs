using API.Abstraction.Master;
using API.Services.Master;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions.Master;

public static class MasterApiServiceCollectionExtensions
{
    public static IServiceCollection AddMasterApiServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitService, UnitService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ITaxMasterService, TaxMasterService>();

        return services;
    }
}
