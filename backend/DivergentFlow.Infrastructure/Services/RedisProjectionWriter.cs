using System.Text.Json;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DivergentFlow.Infrastructure.Services;

/// <summary>
/// Redis implementation of <see cref="IProjectionWriter"/>.
/// Syncs item and collection snapshots to Redis for eventual consistency.
/// All failures are logged but not thrown to prevent blocking primary operations.
/// </summary>
public sealed class RedisProjectionWriter : IProjectionWriter
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisProjectionWriter> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public RedisProjectionWriter(
        IConnectionMultiplexer redis,
        ILogger<RedisProjectionWriter> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task SyncItemAsync(Item item, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = $"item:{item.Id}";
            var json = JsonSerializer.Serialize(item, JsonOptions);
            
            await db.StringSetAsync(key, json);
            
            _logger.LogDebug("Synced item {ItemId} to Redis projection", item.Id);
        }
        catch (Exception ex)
        {
            // Log but don't throw - Redis sync failures should not fail the primary operation
            _logger.LogWarning(
                ex,
                "Failed to sync item {ItemId} to Redis projection. This is non-fatal.",
                item.Id);
        }
    }

    public async Task SyncCollectionAsync(Collection collection, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = $"collection:{collection.Id}";
            var json = JsonSerializer.Serialize(collection, JsonOptions);
            
            await db.StringSetAsync(key, json);
            
            _logger.LogDebug("Synced collection {CollectionId} to Redis projection", collection.Id);
        }
        catch (Exception ex)
        {
            // Log but don't throw - Redis sync failures should not fail the primary operation
            _logger.LogWarning(
                ex,
                "Failed to sync collection {CollectionId} to Redis projection. This is non-fatal.",
                collection.Id);
        }
    }
}
