using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Catalog.Infrastructure;

public static class Dependency
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration["CacheDb:ConnectionString"] ?? "localhost:6379";
    
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<IConnectionMultiplexer>>();
            
            logger.LogInformation("Redis connection string: {RedisConnection}", redisConnection);
        
            var configOptions = ConfigurationOptions.Parse(redisConnection);
            configOptions.AbortOnConnectFail = false;
            configOptions.ConnectRetry = 5;
            configOptions.ConnectTimeout = 10000;

            try
            {
                logger.LogInformation("Connecting to Redis...");
                var multiplexer = ConnectionMultiplexer.Connect(configOptions);
                logger.LogInformation("Redis connected: {IsConnected}", multiplexer.IsConnected);
                return multiplexer;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed redis connection");
                throw;
            }
        });
        
        services.AddSingleton<ICacheRepository>(cr =>
        {
            var connection = cr.GetRequiredService<IConnectionMultiplexer>();
            var defaultExpiration = TimeSpan.FromMinutes(10);

            return new RedisRepository(connection, defaultExpiration);
        });
        
        services.AddScoped(typeof(IDbRepository<,>), typeof(MongoRepository<,>));
        
        return services;
    }
}