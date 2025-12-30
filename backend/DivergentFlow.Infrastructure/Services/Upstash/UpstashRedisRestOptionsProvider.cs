using Microsoft.Extensions.Configuration;

namespace DivergentFlow.Infrastructure.Services.Upstash;

public sealed class UpstashRedisRestOptionsProvider
{
    public UpstashRedisRestOptions? Options { get; }

    public UpstashRedisRestOptionsProvider(IConfiguration configuration)
    {
        // Prefer the dedicated REST URL if provided (matches Upstash console fields).
        var restUrl =
            configuration["UPSTASH_REDIS_REST_URL"]
            ?? configuration["REDIS_REST_URL"]
            ?? configuration["REDIS_URL"]; // back-compat

        var writeToken =
            configuration["UPSTASH_REDIS_REST_TOKEN"]
            ?? configuration["REDIS_TOKEN"];

        var readToken =
            configuration["UPSTASH_REDIS_REST_READONLY_TOKEN"]
            ?? configuration["REDIS_READONLY_TOKEN"];

        if (string.IsNullOrWhiteSpace(restUrl) || string.IsNullOrWhiteSpace(writeToken))
        {
            Options = null;
            return;
        }

        // If this doesn't look like Upstash, don't force REST.
        // Local Docker redis uses StackExchange.Redis.
        var forceRest = bool.TryParse(configuration["REDIS_USE_REST"], out var parsedForceRest) && parsedForceRest;
        if (!restUrl.Contains("upstash.io", StringComparison.OrdinalIgnoreCase) && !forceRest)
        {
            Options = null;
            return;
        }

        var baseUri = UpstashRedisRestUri.BuildBaseUri(restUrl);
        Options = new UpstashRedisRestOptions(
            baseUri,
            writeToken,
            string.IsNullOrWhiteSpace(readToken) ? null : readToken);
    }
}
