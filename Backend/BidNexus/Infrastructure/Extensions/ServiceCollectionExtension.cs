using Core.Abstraction.GlobalData;
using Infrastructure.Repository.GlobalData;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IGlobalDataRepository, GlobalDataRepository>();

        return services;
    }
}
