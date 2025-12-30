using DivergentFlow.Application.Abstractions;
using DivergentFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DivergentFlow.Infrastructure.Repositories;

public sealed class RedisCaptureRepository : ICaptureRepository
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCaptureRepository> _logger;

    public RedisCaptureRepository(IConnectionMultiplexer redis, ILogger<RedisCaptureRepository> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Capture>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        _logger.LogDebug("RedisCaptureRepository.GetAllAsync reading set {SetKey}", RedisCaptureStorage.CapturesSetKey);

        var members = await db.SetMembersAsync(RedisCaptureStorage.CapturesSetKey).ConfigureAwait(false);
        if (members.Length == 0)
        {
            _logger.LogDebug("RedisCaptureRepository.GetAllAsync no ids found");
            return Array.Empty<Capture>();
        }

        var keys = members
            .Select(m => (RedisKey)RedisCaptureStorage.CaptureKey(m.ToString()))
            .ToArray();

        var values = await db.StringGetAsync(keys).ConfigureAwait(false);

        var results = new List<Capture>(values.Length);
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

            var capture = RedisCaptureStorage.Deserialize(value!);
            if (capture is null)
            {
                deserializationFailures++;
                continue;
            }

            results.Add(capture);
        }

        if (missing > 0 || deserializationFailures > 0)
        {
            _logger.LogWarning(
                "RedisCaptureRepository.GetAllAsync had missing={Missing} deserializationFailures={Failures} outOf={Total}",
                missing,
                deserializationFailures,
                values.Length);
        }

        return results;
    }

    public async Task<Capture?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        var key = RedisCaptureStorage.CaptureKey(id);

        _logger.LogDebug("RedisCaptureRepository.GetByIdAsync id={Id} key={Key}", id, key);

        var value = await db.StringGetAsync(key).ConfigureAwait(false);
        if (!value.HasValue)
        {
            _logger.LogDebug("RedisCaptureRepository.GetByIdAsync id={Id} not found", id);
            return null;
        }

        var capture = RedisCaptureStorage.Deserialize(value!);
        if (capture is null)
        {
            _logger.LogWarning("RedisCaptureRepository.GetByIdAsync id={Id} failed to deserialize", id);
        }

        return capture;
    }

    public async Task<Capture> CreateAsync(Capture capture, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        var key = RedisCaptureStorage.CaptureKey(capture.Id);
        var json = RedisCaptureStorage.Serialize(capture);

        _logger.LogDebug(
            "RedisCaptureRepository.CreateAsync id={Id} key={Key} payloadBytes={Bytes}",
            capture.Id,
            key,
            System.Text.Encoding.UTF8.GetByteCount(json));

        var saved = await db.StringSetAsync(key, json).ConfigureAwait(false);
        var indexed = await db.SetAddAsync(RedisCaptureStorage.CapturesSetKey, capture.Id).ConfigureAwait(false);

        _logger.LogDebug(
            "RedisCaptureRepository.CreateAsync id={Id} saved={Saved} indexed={Indexed}",
            capture.Id,
            saved,
            indexed);

        return capture;
    }

    public async Task<Capture?> UpdateAsync(string id, Capture updated, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        var key = RedisCaptureStorage.CaptureKey(id);

        var exists = await db.KeyExistsAsync(key).ConfigureAwait(false);
        if (!exists)
        {
            _logger.LogDebug("RedisCaptureRepository.UpdateAsync id={Id} not found", id);
            return null;
        }

        var json = RedisCaptureStorage.Serialize(updated);
        var saved = await db.StringSetAsync(key, json).ConfigureAwait(false);
        var indexed = await db.SetAddAsync(RedisCaptureStorage.CapturesSetKey, id).ConfigureAwait(false);

        _logger.LogDebug(
            "RedisCaptureRepository.UpdateAsync id={Id} saved={Saved} indexed={Indexed}",
            id,
            saved,
            indexed);

        return updated;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var db = _redis.GetDatabase();
        var key = RedisCaptureStorage.CaptureKey(id);

        var deleted = await db.KeyDeleteAsync(key).ConfigureAwait(false);
        var deindexed = await db.SetRemoveAsync(RedisCaptureStorage.CapturesSetKey, id).ConfigureAwait(false);

        _logger.LogDebug(
            "RedisCaptureRepository.DeleteAsync id={Id} deleted={Deleted} deindexed={Deindexed}",
            id,
            deleted,
            deindexed);

        return deleted;
    }

    public async Task<IReadOnlyList<Capture>> GetCapturesNeedingReInferenceAsync(
        double confidenceThreshold,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Get all captures and filter in memory
        var allCaptures = await GetAllAsync(cancellationToken).ConfigureAwait(false);

        // Filter for non-migrated captures with null confidence or confidence below threshold
        var capturesNeedingInference = allCaptures
            .Where(c => !c.IsMigrated && 
                       (c.TypeConfidence == null || c.TypeConfidence < confidenceThreshold))
            .ToList();

        _logger.LogDebug(
            "RedisCaptureRepository.GetCapturesNeedingReInferenceAsync found {Count} captures (threshold={Threshold})",
            capturesNeedingInference.Count,
            confidenceThreshold);

        return capturesNeedingInference;
    }
}
