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

    public void TriggerInferenceWorkflow(string captureId)
    {
        // Enqueue asynchronously without blocking the caller.
        var enqueueTask = _queue.EnqueueAsync(captureId);

        // Log success when the enqueue operation completes.
        enqueueTask.ContinueWith(
            _ => _logger.LogDebug(
                "Type inference workflow triggered for item {ItemId}. Enqueued for background processing.",
                captureId),
            TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);

        // Log failures so that exceptions are observed and not lost.
        enqueueTask.ContinueWith(
            t => _logger.LogError(
                t.Exception,
                "Failed to enqueue item {ItemId} for type inference",
                captureId),
            TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
    }
}
