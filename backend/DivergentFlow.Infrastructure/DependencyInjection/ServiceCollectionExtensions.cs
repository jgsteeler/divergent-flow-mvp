using DivergentFlow.Application.Abstractions;
using DivergentFlow.Infrastructure.Repositories;
using DivergentFlow.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DivergentFlow.Infrastructure.DependencyInjection;

/// <summary>
/// Provides extension methods for registering infrastructure-layer services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers infrastructure-layer services, including repository and type inference implementations.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which infrastructure services are added.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance to allow for method chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Redis");

            var connectionString =
                configuration["REDIS_CONNECTION_STRING"]
                ?? configuration.GetConnectionString("Redis")
                ?? configuration["Redis:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                // Back-compat with previous config style (used by the old Services project):
                // - REDIS_URL: host:port (or may include scheme)
                // - REDIS_TOKEN: password/token (optional for local)
                var redisUrl = configuration["REDIS_URL"];
                var redisToken = configuration["REDIS_TOKEN"];

                if (!string.IsNullOrWhiteSpace(redisUrl))
                {
                    var url = redisUrl
                        .Replace("http://", string.Empty, StringComparison.OrdinalIgnoreCase)
                        .Replace("https://", string.Empty, StringComparison.OrdinalIgnoreCase)
                        .Replace("redis://", string.Empty, StringComparison.OrdinalIgnoreCase)
                        .Replace("rediss://", string.Empty, StringComparison.OrdinalIgnoreCase);

                    var ssl = false;

                    // Heuristic: Upstash typically requires TLS.
                    if (redisUrl.StartsWith("rediss://", StringComparison.OrdinalIgnoreCase)
                        || url.Contains("upstash.io", StringComparison.OrdinalIgnoreCase))
                    {
                        ssl = true;
                    }

                    // Allow explicit override via env var.
                    if (bool.TryParse(configuration["REDIS_SSL"], out var sslOverride))
                    {
                        ssl = sslOverride;
                    }

                    // StackExchange.Redis connection string format:
                    // host:port[,user=default][,password=...][,ssl=True/False]
                    // Don't log the token.
                    connectionString = string.IsNullOrWhiteSpace(redisToken)
                        ? $"{url},ssl={ssl}" 
                        : $"{url},user=default,password={redisToken},ssl={ssl}";

                    logger.LogInformation(
                        "Built Redis connection string from REDIS_URL (ssl={Ssl})",
                        ssl);
                }
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Redis connection string not configured. Set REDIS_CONNECTION_STRING, or REDIS_URL (+ optional REDIS_TOKEN), or ConnectionStrings:Redis / Redis:ConnectionString.");
            }

            var options = ConfigurationOptions.Parse(connectionString);
            options.AbortOnConnectFail = false;

            logger.LogInformation(
                "Connecting to Redis. endpoints={Endpoints}",
                string.Join(",", options.EndPoints.Select(e => e.ToString())));

            return ConnectionMultiplexer.Connect(options);
        });

        services.AddScoped<ICaptureRepository, RedisCaptureRepository>();
        services.AddSingleton<ITypeInferenceService, BasicTypeInferenceService>();
        return services;
    }
}
