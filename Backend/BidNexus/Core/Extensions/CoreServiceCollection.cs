using Core.Abstraction.Services;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Extensions
{
    public static class CoreServiceCollection
    {
        public static IServiceCollection CoreServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IJwtHelperService, JwtHelperService>();
            return services;
        }
    }
}
