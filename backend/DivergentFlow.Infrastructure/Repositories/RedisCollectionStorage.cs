using System.Text.Json;
using System.Text.Json.Serialization;
using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Infrastructure.Repositories;

public static class RedisCollectionStorage
{
    public const string CollectionsSetKey = "collections:ids";

    public static string CollectionKey(string id) => $"collection:{id}";

    public static string Serialize(Collection collection)
    {
        return JsonSerializer.Serialize(collection, SerializerOptions);
    }

    public static Collection? Deserialize(string json)
    {
        return JsonSerializer.Deserialize<Collection>(json, SerializerOptions);
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
