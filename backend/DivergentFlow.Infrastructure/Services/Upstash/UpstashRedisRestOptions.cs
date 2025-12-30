namespace DivergentFlow.Infrastructure.Services.Upstash;

public sealed record UpstashRedisRestOptions(
    Uri BaseUri,
    string WriteToken,
    string? ReadToken
);
