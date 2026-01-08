namespace DivergentFlow.Application.Abstractions;

/// <summary>
/// Represents a single queued inference work item.
/// </summary>
public sealed record InferenceQueueWorkItem(string UserId, string ItemId);

/// <summary>
/// Defines the contract for queuing items for background type inference processing.
/// </summary>
public interface IInferenceQueue
{
    /// <summary>
    /// Enqueues an item for type inference processing.
    /// This is a fire-and-forget operation that does not block.
    /// </summary>
    /// <param name="userId">The user identifier that owns the item.</param>
    /// <param name="itemId">The ID of the item to process.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the item has been queued.</returns>
    ValueTask EnqueueAsync(string userId, string itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dequeues the next item ID to process.
    /// This operation blocks until an item is available or cancellation is requested.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The next queued work item to process.</returns>
    ValueTask<InferenceQueueWorkItem> DequeueAsync(CancellationToken cancellationToken = default);
}
