using Microsoft.Extensions.Configuration;

namespace DivergentFlow.Infrastructure.Services.Upstash;

public sealed class UpstashRedisRestOptionsProvider
{
    public UpstashRedisRestOptions? Options { get; }

    public UpstashRedisRestOptionsProvider(IConfiguration configuration)
    {
        var restUrl = configuration["UPSTASH_REDIS_REST_URL"];
        var writeToken = configuration["UPSTASH_REDIS_REST_TOKEN"];
        var readToken = configuration["UPSTASH_REDIS_REST_READONLY_TOKEN"];

        if (string.IsNullOrWhiteSpace(restUrl) || string.IsNullOrWhiteSpace(writeToken))
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
