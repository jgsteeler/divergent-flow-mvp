using DivergentFlow.Application.Abstractions;
using DivergentFlow.Infrastructure.Repositories;
using DivergentFlow.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Net;

namespace DivergentFlow.Infrastructure.DependencyInjection;

/// <summary>
/// Provides extension methods for registering infrastructure-layer services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    internal static ConfigurationOptions BuildRedisOptions(
        string baseConnectionString,
        string? redisToken,
        string? redisSslRaw)
    {
        // StackExchange.Redis supports its own connection-string format.
        // However, many hosted providers (e.g. Upstash) surface a redis/rediss URL.
        // Passing a URL directly to ConfigurationOptions.Parse can result in malformed endpoints like
        // "rediss://...:6379:6379". We detect URLs and parse them ourselves.

        ConfigurationOptions options;

        if (TryParseRedisUri(baseConnectionString, out var uri))
        {
            var port = uri.IsDefaultPort ? 6379 : uri.Port;

            options = new ConfigurationOptions
            {
                Ssl = uri.Scheme.Equals("rediss", StringComparison.OrdinalIgnoreCase),
                SslHost = uri.Host
            };

            options.EndPoints.Add(uri.Host, port);

            // Parse userinfo: "user:password" OR ":password" OR "password".
            if (!string.IsNullOrWhiteSpace(uri.UserInfo))
            {
                var parts = uri.UserInfo.Split(':', 2, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    if (!string.IsNullOrWhiteSpace(parts[0]))
                    {
                        options.User = Uri.UnescapeDataString(parts[0]);
                    }

                    if (!string.IsNullOrWhiteSpace(parts[1]))
                    {
                        options.Password = Uri.UnescapeDataString(parts[1]);
                    }
                }
                else if (parts.Length == 1)
                {
                    // Some tooling may place the password alone in userinfo.
                    options.Password = Uri.UnescapeDataString(parts[0]);
                }
            }
        }
        else
        {
            options = ConfigurationOptions.Parse(baseConnectionString);
        }

        var hasUpstashEndpoint = options.EndPoints
            .Select(ep => ep as DnsEndPoint)
            .Where(ep => ep is not null)
            .Select(ep => ep!.Host)
            .Any(host => host.Contains("upstash.io", StringComparison.OrdinalIgnoreCase));

        // Determine whether SSL should be enabled.
        var ssl = options.Ssl;
        if (bool.TryParse(redisSslRaw, out var sslOverride))
        {
            ssl = sslOverride;
        }
        else if (hasUpstashEndpoint)
        {
            ssl = true;
        }

        options.Ssl = ssl;

        // Ensure SNI host is set when using TLS (common requirement for hosted Redis).
        if (options.Ssl && string.IsNullOrWhiteSpace(options.SslHost))
        {
            var host = options.EndPoints
                .Select(ep => ep as DnsEndPoint)
                .Where(ep => ep is not null)
                .Select(ep => ep!.Host)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(host))
            {
                options.SslHost = host;
            }
        }

        // If token is provided separately, apply it unless already present in the base connection string.
        if (!string.IsNullOrWhiteSpace(redisToken) && string.IsNullOrWhiteSpace(options.Password))
        {
            options.Password = redisToken;
        }

        // Upstash commonly requires the default ACL user.
        if (!string.IsNullOrWhiteSpace(options.Password) && string.IsNullOrWhiteSpace(options.User))
        {
            options.User = "default";
        }

        options.AbortOnConnectFail = false;
        return options;
    }

    private static bool TryParseRedisUri(string value, out Uri uri)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out uri!))
        {
            return false;
        }

        if (uri.Scheme is not ("redis" or "rediss"))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(uri.Host))
        {
            return false;
        }

        return true;
    }

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

            var rawConnectionString =
                configuration["REDIS_CONNECTION_STRING"]
                ?? configuration.GetConnectionString("Redis")
                ?? configuration["Redis:ConnectionString"];

            // Back-compat with previous config style (used by the old Services project):
            // - REDIS_URL: host:port OR a full redis/rediss URL
            // - REDIS_TOKEN: password/token (optional for local)
            var redisUrl = configuration["REDIS_URL"];
            var redisToken = configuration["REDIS_TOKEN"];

            // Prefer explicit connection string values, else fall back to REDIS_URL.
            var baseConnectionString = !string.IsNullOrWhiteSpace(rawConnectionString)
                ? rawConnectionString
                : redisUrl;

            if (string.IsNullOrWhiteSpace(baseConnectionString))
            {
                throw new InvalidOperationException(
                    "Redis connection string not configured. Set REDIS_CONNECTION_STRING, or REDIS_URL (+ optional REDIS_TOKEN), or ConnectionStrings:Redis / Redis:ConnectionString.");
            }

            var options = BuildRedisOptions(baseConnectionString, redisToken, configuration["REDIS_SSL"]);

            logger.LogInformation(
                "Connecting to Redis. endpoints={Endpoints} ssl={Ssl} userConfigured={UserConfigured}",
                string.Join(",", options.EndPoints.Select(e => e.ToString())),
                options.Ssl,
                !string.IsNullOrWhiteSpace(options.User));

            return ConnectionMultiplexer.Connect(options);
        });

        services.AddScoped<ICaptureRepository, RedisCaptureRepository>();
        services.AddSingleton<ITypeInferenceService, BasicTypeInferenceService>();
        return services;
    }
}
