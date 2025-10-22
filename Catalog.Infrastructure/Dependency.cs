using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Catalog.Infrastructure;

public static class Dependency
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration["CacheDb.ConnectionString"] ??  "localhost:6379";
        
        services.AddSingleton<IConnectionMultiplexer>(conn =>
        {
            var configOptions = ConfigurationOptions.Parse(redisConnection);
            return ConnectionMultiplexer.Connect(configOptions);
        });
        
        services.AddSingleton<ICacheRepository>(cr =>
        {
            var connection = cr.GetRequiredService<IConnectionMultiplexer>();
            var defaultExpiration = TimeSpan.FromMinutes(10);

            return new RedisRepository(connection, defaultExpiration);
        });
        
        services.AddScoped(typeof(IDbRepository<>), typeof(MongoRepository<>));
        
        return services;
    }
}