using Catalog.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CatalogService>();
        services.AddScoped<BrandService>();
        services.AddScoped<TypeService>();
        
        return services;
    }
}