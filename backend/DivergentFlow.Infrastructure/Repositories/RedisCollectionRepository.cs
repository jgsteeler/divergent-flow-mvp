using DivergentFlow.Application.Abstractions;
using DivergentFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DivergentFlow.Infrastructure.Repositories;

public sealed class RedisCollectionRepository : ICollectionRepository
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCollectionRepository> _logger;

    public RedisCollectionRepository(IConnectionMultiplexer redis, ILogger<RedisCollectionRepository> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Collection>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        _logger.LogDebug("RedisCollectionRepository.GetAllAsync reading set {SetKey}", RedisCollectionStorage.CollectionsSetKey);

        var members = await db.SetMembersAsync(RedisCollectionStorage.CollectionsSetKey).ConfigureAwait(false);
        if (members.Length == 0)
        {
            _logger.LogDebug("RedisCollectionRepository.GetAllAsync no ids found");
            return Array.Empty<Collection>();
        }

        var keys = members
            .Select(m => (RedisKey)RedisCollectionStorage.CollectionKey(m.ToString()))
            .ToArray();

        var values = await db.StringGetAsync(keys).ConfigureAwait(false);

        var results = new List<Collection>(values.Length);
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

            var collection = RedisCollectionStorage.Deserialize(value!);
            if (collection is null)
            {
                deserializationFailures++;
                continue;
            }

            results.Add(collection);
        }

        if (missing > 0 || deserializationFailures > 0)
        {
            _logger.LogWarning(
                "RedisCollectionRepository.GetAllAsync had missing={Missing} deserializationFailures={Failures} outOf={Total}",
                missing,
                deserializationFailures,
                values.Length);
        }

        return results;
    }

    public async Task<Collection?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        var key = RedisCollectionStorage.CollectionKey(id);

        _logger.LogDebug("RedisCollectionRepository.GetByIdAsync id={Id} key={Key}", id, key);

        var value = await db.StringGetAsync(key).ConfigureAwait(false);
        if (!value.HasValue)
        {
            _logger.LogDebug("RedisCollectionRepository.GetByIdAsync id={Id} not found", id);
            return null;
        }

        var collection = RedisCollectionStorage.Deserialize(value!);
        if (collection is null)
        {
            _logger.LogWarning("RedisCollectionRepository.GetByIdAsync id={Id} failed to deserialize", id);
        }

        return collection;
    }

    public async Task<Collection> CreateAsync(Collection collection, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        var key = RedisCollectionStorage.CollectionKey(collection.Id);
        var json = RedisCollectionStorage.Serialize(collection);

        _logger.LogDebug(
            "RedisCollectionRepository.CreateAsync id={Id} key={Key} payloadBytes={Bytes}",
            collection.Id,
            key,
            System.Text.Encoding.UTF8.GetByteCount(json));

        var saved = await db.StringSetAsync(key, json).ConfigureAwait(false);
        var indexed = await db.SetAddAsync(RedisCollectionStorage.CollectionsSetKey, collection.Id).ConfigureAwait(false);

        _logger.LogDebug(
            "RedisCollectionRepository.CreateAsync id={Id} saved={Saved} indexed={Indexed}",
            collection.Id,
            saved,
            indexed);

        return collection;
    }

    public async Task<Collection?> UpdateAsync(string id, Collection updated, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        var key = RedisCollectionStorage.CollectionKey(id);

        var exists = await db.KeyExistsAsync(key).ConfigureAwait(false);
        if (!exists)
        {
            _logger.LogDebug("RedisCollectionRepository.UpdateAsync id={Id} not found", id);
            return null;
        }

        var json = RedisCollectionStorage.Serialize(updated);
        var saved = await db.StringSetAsync(key, json).ConfigureAwait(false);
        var indexed = await db.SetAddAsync(RedisCollectionStorage.CollectionsSetKey, id).ConfigureAwait(false);

        _logger.LogDebug(
            "RedisCollectionRepository.UpdateAsync id={Id} saved={Saved} indexed={Indexed}",
            id,
            saved,
            indexed);

        return updated;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        var key = RedisCollectionStorage.CollectionKey(id);

        var deleted = await db.KeyDeleteAsync(key).ConfigureAwait(false);
        var deindexed = await db.SetRemoveAsync(RedisCollectionStorage.CollectionsSetKey, id).ConfigureAwait(false);

        _logger.LogDebug(
            "RedisCollectionRepository.DeleteAsync id={Id} deleted={Deleted} deindexed={Deindexed}",
            id,
            deleted,
            deindexed);

        return deleted;
    }
}
