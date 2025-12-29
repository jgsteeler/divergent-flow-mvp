using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Redis.OM;

namespace DivergentFlow.Services.Extensions;

/// <summary>
/// Extension methods for registering Redis-related services
/// </summary>
public static class RedisServiceExtensions
{
    /// <summary>
    /// Registers Redis connection provider and related services
    /// </summary>
    public static IServiceCollection AddRedisServices(this IServiceCollection services)
    {
        // Register RedisConnectionProvider as singleton
        services.AddSingleton(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var logger = provider.GetRequiredService<ILogger<RedisConnectionProvider>>();

            // Get Redis connection string from environment variables
            var redisUrl = configuration["REDIS_URL"]
                ?? throw new InvalidOperationException("REDIS_URL environment variable is not set");

            var redisToken = configuration["REDIS_TOKEN"] ?? string.Empty;

            // Clean up URL - remove http:// or https:// if present
            redisUrl = redisUrl.Replace("http://", "").Replace("https://", "");

            // Build connection string with authentication
            // Upstash Redis format: redis://default:{token}@{host}:{port}
            // Local Redis format (no auth): redis://{host}:{port}
            var connectionString = string.IsNullOrEmpty(redisToken)
                ? $"redis://{redisUrl}"
                : $"redis://default:{redisToken}@{redisUrl}";

            logger.LogInformation("Connecting to Redis at {RedisUrl}", redisUrl);

            var connectionProvider = new RedisConnectionProvider(connectionString);

            // Create index if it doesn't exist
            try
            {
                connectionProvider.Connection.CreateIndex(typeof(Models.CaptureEntity));
                logger.LogInformation("Redis index created or already exists for CaptureEntity");
            }
            catch (Exception ex)
            {
                // Index might already exist, which is fine
                logger.LogDebug(ex, "Index creation attempt (may already exist)");
            }

            return connectionProvider;
        });

        return services;
    }
}
