using System.Text.Json;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Domain.Entities;
using DivergentFlow.Infrastructure.Services.Upstash;
using Microsoft.Extensions.Logging;

namespace DivergentFlow.Infrastructure.Services;

/// <summary>
/// Redis implementation of <see cref="IProjectionWriter"/>.
/// Syncs item and collection snapshots to Redis for eventual consistency.
/// All failures are logged but not thrown to prevent blocking primary operations.
/// </summary>
public sealed class RedisProjectionWriter : IProjectionWriter
{
    private readonly IUpstashRedisRestWriteClient _write;
    private readonly ILogger<RedisProjectionWriter> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public RedisProjectionWriter(
        IUpstashRedisRestWriteClient write,
        ILogger<RedisProjectionWriter> logger)
    {
        _write = write;
        _logger = logger;
    }

    public async Task SyncItemAsync(Item item, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = $"item:{item.Id}";
            var json = JsonSerializer.Serialize(item, JsonOptions);

            await _write.SetAsync(key, json, cancellationToken).ConfigureAwait(false);
            
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
            var key = $"collection:{collection.Id}";
            var json = JsonSerializer.Serialize(collection, JsonOptions);

            await _write.SetAsync(key, json, cancellationToken).ConfigureAwait(false);
            
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
