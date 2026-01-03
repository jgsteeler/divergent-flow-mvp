using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Application.Abstractions;

/// <summary>
/// Defines the contract for syncing item and collection snapshots to Redis as a projection cache.
/// Redis sync failures should not fail the primary operation.
/// </summary>
public interface IProjectionWriter
{
    /// <summary>
    /// Syncs an item snapshot to Redis.
    /// This operation is idempotent and should not throw on failure.
    /// </summary>
    /// <param name="item">The item to sync.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the sync attempt is done (success or failure).</returns>
    Task SyncItemAsync(Item item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Syncs a collection snapshot to Redis.
    /// This operation is idempotent and should not throw on failure.
    /// </summary>
    /// <param name="collection">The collection to sync.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the sync attempt is done (success or failure).</returns>
    Task SyncCollectionAsync(Collection collection, CancellationToken cancellationToken = default);
}
