using DivergentFlow.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace DivergentFlow.Application.Services;

/// <summary>
/// Implementation of the type inference workflow trigger that enqueues items for processing.
/// Items are processed asynchronously by the InferenceQueueProcessorService.
/// </summary>
public sealed class SimpleTypeInferenceWorkflowTrigger : ITypeInferenceWorkflowTrigger
{
    private readonly IInferenceQueue _queue;
    private readonly ILogger<SimpleTypeInferenceWorkflowTrigger> _logger;

    public SimpleTypeInferenceWorkflowTrigger(
        IInferenceQueue queue,
        ILogger<SimpleTypeInferenceWorkflowTrigger> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    public void TriggerInferenceWorkflow(string userId, string itemId)
    {
        // Fire and forget - enqueue asynchronously without blocking
        _ = EnqueueAsync(userId, itemId);
    }

    private async Task EnqueueAsync(string userId, string itemId)
    {
        try
        {
            await _queue.EnqueueAsync(userId, itemId);
            _logger.LogDebug(
                "Type inference workflow triggered for item {ItemId} (userId={UserId}). Enqueued for background processing.",
                itemId,
                userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to enqueue item {ItemId} for type inference (userId={UserId})",
                itemId,
                userId);
        }
    }
}
