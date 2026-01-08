using System.Text.Json;
using System.Text.Json.Serialization;
using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Infrastructure.Repositories;

public static class RedisCaptureStorage
{
    public const string CapturesSetKey = "captures:ids";

    public static string CaptureKey(string id) => $"capture:{id}";

    public static string CapturesSetKeyForUser(string userId)
        => IsLocal(userId) ? CapturesSetKey : $"users:{userId}:captures:ids";

    public static string CaptureKeyForUser(string userId, string id)
        => IsLocal(userId) ? CaptureKey(id) : $"users:{userId}:capture:{id}";

    private static bool IsLocal(string userId)
        => string.Equals(userId, "local", StringComparison.Ordinal);

    public static string Serialize(Capture capture)
    {
        return JsonSerializer.Serialize(capture, SerializerOptions);
    }

    public static Capture? Deserialize(string json)
    {
        return JsonSerializer.Deserialize<Capture>(json, SerializerOptions);
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
