using System.Text.Json;

namespace DivergentFlow.Infrastructure.Services.Upstash;

internal sealed record UpstashRedisRestResponse(JsonElement? Result, string? Error);
