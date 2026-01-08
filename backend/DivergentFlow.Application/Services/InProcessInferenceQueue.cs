using System.Threading.Channels;
using DivergentFlow.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace DivergentFlow.Application.Services;

/// <summary>
/// In-process queue for background type inference processing using System.Threading.Channels.
/// This provides an efficient, thread-safe queue for asynchronous processing.
/// </summary>
public sealed class InProcessInferenceQueue : IInferenceQueue
{
    private readonly Channel<InferenceQueueWorkItem> _channel;
    private readonly ILogger<InProcessInferenceQueue> _logger;

    public InProcessInferenceQueue(ILogger<InProcessInferenceQueue> logger)
    {
        _logger = logger;
        
        // Create an unbounded channel for simplicity in MVP
        // In production, consider a bounded channel with appropriate capacity
        _channel = Channel.CreateUnbounded<InferenceQueueWorkItem>(new UnboundedChannelOptions
        {
            SingleReader = true, // Only one background worker reads
            SingleWriter = false // Multiple API requests can write
        });
        
        _logger.LogInformation("InProcessInferenceQueue initialized");
    }

    public async ValueTask EnqueueAsync(string userId, string itemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var workItem = new InferenceQueueWorkItem(userId, itemId);
            await _channel.Writer.WriteAsync(workItem, cancellationToken);
            _logger.LogDebug(
                "Enqueued item {ItemId} for inference processing (userId={UserId})",
                itemId,
                userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error enqueuing item {ItemId} for inference processing (userId={UserId})",
                itemId,
                userId);
            throw;
        }
    }

    public async ValueTask<InferenceQueueWorkItem> DequeueAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var workItem = await _channel.Reader.ReadAsync(cancellationToken);
            _logger.LogDebug(
                "Dequeued item {ItemId} for inference processing (userId={UserId})",
                workItem.ItemId,
                workItem.UserId);
            return workItem;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Dequeue operation cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dequeuing item for inference processing");
            throw;
        }
    }
}
