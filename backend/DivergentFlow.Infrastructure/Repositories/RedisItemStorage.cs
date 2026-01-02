using System.Text.Json;
using System.Text.Json.Serialization;
using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Infrastructure.Repositories;

public static class RedisItemStorage
{
    public const string ItemsSetKey = "items:ids";

    public static string ItemKey(string id) => $"item:{id}";

    public static string Serialize(Item item)
    {
        return JsonSerializer.Serialize(item, SerializerOptions);
    }

    public static Item? Deserialize(string json)
    {
        return JsonSerializer.Deserialize<Item>(json, SerializerOptions);
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
