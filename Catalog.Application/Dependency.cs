using Catalog.Application.Interfaces;
using Catalog.Application.Services;
using Catalog.Common.Dtos;
using Catalog.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

public static class Dependency
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICacheService, RedisCacheService>();
        
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ITypeService, TypeService>();
        
        return services;
    }
}