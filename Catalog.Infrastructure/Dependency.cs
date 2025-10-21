using Catalog.Common.Models;
using Catalog.Infrastructure.Context;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure;

public static class Dependency
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));
        
        return services;
    }
}