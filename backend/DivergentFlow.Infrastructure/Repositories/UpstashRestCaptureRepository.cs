using DivergentFlow.Application.Abstractions;
using DivergentFlow.Domain.Entities;
using DivergentFlow.Infrastructure.Services.Upstash;
using Microsoft.Extensions.Logging;

namespace DivergentFlow.Infrastructure.Repositories;

public sealed class UpstashRestCaptureRepository : ICaptureRepository
{
    private readonly IUpstashRedisRestReadClient _read;
    private readonly IUpstashRedisRestWriteClient _write;
    private readonly ILogger<UpstashRestCaptureRepository> _logger;

    public UpstashRestCaptureRepository(
        IUpstashRedisRestReadClient read,
        IUpstashRedisRestWriteClient write,
        ILogger<UpstashRestCaptureRepository> logger)
    {
        _read = read;
        _write = write;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Capture>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var ids = await _read.SMembersAsync(
            RedisCaptureStorage.CapturesSetKeyForUser(userId),
            cancellationToken).ConfigureAwait(false);
        if (ids.Length == 0)
        {
            return Array.Empty<Capture>();
        }

        var keys = ids.Select(id => RedisCaptureStorage.CaptureKeyForUser(userId, id)).ToArray();
        var values = await _read.MGetAsync(keys, cancellationToken).ConfigureAwait(false);

        var results = new List<Capture>(values.Count);
        var missing = 0;
        var failures = 0;

        foreach (var value in values)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(value))
            {
                missing++;
                continue;
            }

            var capture = RedisCaptureStorage.Deserialize(value);
            if (capture is null)
            {
                failures++;
                continue;
            }

            results.Add(capture);
        }

        if (missing > 0 || failures > 0)
        {
            _logger.LogWarning(
                "UpstashRestCaptureRepository.GetAllAsync missing={Missing} failures={Failures} outOf={Total}",
                missing,
                failures,
                values.Count);
        }

        return results;
    }

    public async Task<Capture?> GetByIdAsync(string userId, string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = RedisCaptureStorage.CaptureKeyForUser(userId, id);
        var json = await _read.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return RedisCaptureStorage.Deserialize(json);
    }

    public async Task<Capture> CreateAsync(string userId, Capture capture, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        capture.UserId = userId;
        var key = RedisCaptureStorage.CaptureKeyForUser(userId, capture.Id);
        var json = RedisCaptureStorage.Serialize(capture);

        await _write.ExecuteTransactionAsync(
            new List<IReadOnlyList<object?>>
            {
                new object?[] { "SET", key, json },
                new object?[] { "SADD", RedisCaptureStorage.CapturesSetKeyForUser(userId), capture.Id }
            },
            cancellationToken).ConfigureAwait(false);

        return capture;
    }

    public async Task<Capture?> UpdateAsync(string userId, string id, Capture updated, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        updated.UserId = userId;
        var key = RedisCaptureStorage.CaptureKeyForUser(userId, id);
        var existing = await _read.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(existing))
        {
            return null;
        }

        var json = RedisCaptureStorage.Serialize(updated);
        await _write.ExecuteTransactionAsync(
            new List<IReadOnlyList<object?>>
            {
                new object?[] { "SET", key, json },
                new object?[] { "SADD", RedisCaptureStorage.CapturesSetKeyForUser(userId), id }
            },
            cancellationToken).ConfigureAwait(false);

        return updated;
    }

    public async Task<bool> DeleteAsync(string userId, string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = RedisCaptureStorage.CaptureKeyForUser(userId, id);

        // Not strictly atomic to compute the return value, but we still perform both operations.
        var deleted = await _write.DelAsync(key, cancellationToken).ConfigureAwait(false);
        await _write.SRemAsync(RedisCaptureStorage.CapturesSetKeyForUser(userId), id, cancellationToken).ConfigureAwait(false);

        return deleted > 0;
    }

    public async Task<IReadOnlyList<Capture>> GetCapturesNeedingReInferenceAsync(
        string userId,
        double confidenceThreshold,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // TODO: For better scalability, implement database-level filtering using Redis queries
        // or indexing to avoid loading all captures into memory. This MVP implementation
        // fetches all and filters in-memory, which is acceptable for small datasets but
        // should be optimized before reaching production scale.
        
        // Get all captures and filter in memory
        var allCaptures = await GetAllAsync(userId, cancellationToken).ConfigureAwait(false);

        // Filter for non-migrated captures with null confidence or confidence below threshold
        var capturesNeedingInference = allCaptures
            .Where(c => !c.IsMigrated && 
                       (c.TypeConfidence == null || c.TypeConfidence < confidenceThreshold))
            .ToList();

        return capturesNeedingInference;
    }
}
