using DivergentFlow.Application.Abstractions;
using DivergentFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DivergentFlow.Infrastructure.Repositories;

public sealed class RedisItemRepository : IItemRepository
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisItemRepository> _logger;

    public RedisItemRepository(IConnectionMultiplexer redis, ILogger<RedisItemRepository> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        _logger.LogDebug("RedisItemRepository.GetAllAsync reading set {SetKey}", RedisItemStorage.ItemsSetKey);

        var members = await db.SetMembersAsync(RedisItemStorage.ItemsSetKey).ConfigureAwait(false);
        if (members.Length == 0)
        {
            _logger.LogDebug("RedisItemRepository.GetAllAsync no ids found");
            return Array.Empty<Item>();
        }

        var keys = members
            .Select(m => (RedisKey)RedisItemStorage.ItemKey(m.ToString()))
            .ToArray();

        var values = await db.StringGetAsync(keys).ConfigureAwait(false);

        var results = new List<Item>(values.Length);
        var missing = 0;
        var deserializationFailures = 0;

        for (var i = 0; i < values.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var value = values[i];
            if (!value.HasValue)
            {
                missing++;
                continue;
            }

            var item = RedisItemStorage.Deserialize(value!);
            if (item is null)
            {
                deserializationFailures++;
                continue;
            }

            results.Add(item);
        }

        if (missing > 0 || deserializationFailures > 0)
        {
            _logger.LogWarning(
                "RedisItemRepository.GetAllAsync had missing={Missing} deserializationFailures={Failures} outOf={Total}",
                missing,
                deserializationFailures,
                values.Length);
        }

        return results;
    }

    public async Task<Item?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        var key = RedisItemStorage.ItemKey(id);

        _logger.LogDebug("RedisItemRepository.GetByIdAsync id={Id} key={Key}", id, key);

        var value = await db.StringGetAsync(key).ConfigureAwait(false);
        if (!value.HasValue)
        {
            _logger.LogDebug("RedisItemRepository.GetByIdAsync id={Id} not found", id);
            return null;
        }

        var item = RedisItemStorage.Deserialize(value!);
        if (item is null)
        {
            _logger.LogWarning("RedisItemRepository.GetByIdAsync id={Id} failed to deserialize", id);
        }

        return item;
    }

    public async Task<Item> CreateAsync(Item item, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        var key = RedisItemStorage.ItemKey(item.Id);
        var json = RedisItemStorage.Serialize(item);

        _logger.LogDebug(
            "RedisItemRepository.CreateAsync id={Id} key={Key} payloadBytes={Bytes}",
            item.Id,
            key,
            System.Text.Encoding.UTF8.GetByteCount(json));

        var saved = await db.StringSetAsync(key, json).ConfigureAwait(false);
        var indexed = await db.SetAddAsync(RedisItemStorage.ItemsSetKey, item.Id).ConfigureAwait(false);

        _logger.LogDebug(
            "RedisItemRepository.CreateAsync id={Id} saved={Saved} indexed={Indexed}",
            item.Id,
            saved,
            indexed);

        return item;
    }

    public async Task<Item?> UpdateAsync(string id, Item updated, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        var key = RedisItemStorage.ItemKey(id);

        var exists = await db.KeyExistsAsync(key).ConfigureAwait(false);
        if (!exists)
        {
            _logger.LogDebug("RedisItemRepository.UpdateAsync id={Id} not found", id);
            return null;
        }

        var json = RedisItemStorage.Serialize(updated);
        var saved = await db.StringSetAsync(key, json).ConfigureAwait(false);
        var indexed = await db.SetAddAsync(RedisItemStorage.ItemsSetKey, id).ConfigureAwait(false);

        _logger.LogDebug(
            "RedisItemRepository.UpdateAsync id={Id} saved={Saved} indexed={Indexed}",
            id,
            saved,
            indexed);

        return updated;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        var key = RedisItemStorage.ItemKey(id);

        var deleted = await db.KeyDeleteAsync(key).ConfigureAwait(false);
        var deindexed = await db.SetRemoveAsync(RedisItemStorage.ItemsSetKey, id).ConfigureAwait(false);

        _logger.LogDebug(
            "RedisItemRepository.DeleteAsync id={Id} deleted={Deleted} deindexed={Deindexed}",
            id,
            deleted,
            deindexed);

        return deleted;
    }

    public async Task<IReadOnlyList<Item>> GetItemsNeedingReInferenceAsync(
        double confidenceThreshold,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // TODO: For better scalability, implement database-level filtering using Redis queries
        // or indexing to avoid loading all items into memory. This MVP implementation
        // fetches all and filters in-memory, which is acceptable for small datasets but
        // should be optimized before reaching production scale.
        
        // Get all items and filter in memory
        var allItems = await GetAllAsync(cancellationToken).ConfigureAwait(false);

        // Filter for items with null confidence or confidence below threshold
        var itemsNeedingInference = allItems
            .Where(i => i.TypeConfidence == null || i.TypeConfidence < confidenceThreshold)
            .ToList();

        _logger.LogDebug(
            "RedisItemRepository.GetItemsNeedingReInferenceAsync found {Count} items (threshold={Threshold})",
            itemsNeedingInference.Count,
            confidenceThreshold);

        return itemsNeedingInference;
    }
}
