using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DivergentFlow.Application.Services;

/// <summary>
/// Background service that processes items from the inference queue.
/// Loads items from MongoDB, runs type inference, updates MongoDB, and syncs to Redis.
/// </summary>
public sealed class InferenceQueueProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IInferenceQueue _queue;
    private readonly ILogger<InferenceQueueProcessorService> _logger;
    private readonly TypeInferenceOptions _options;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public InferenceQueueProcessorService(
        IServiceProvider serviceProvider,
        IInferenceQueue queue,
        ILogger<InferenceQueueProcessorService> logger,
        IOptions<TypeInferenceOptions> options,
        IHostApplicationLifetime applicationLifetime)
    {
        _serviceProvider = serviceProvider;
        _queue = queue;
        _logger = logger;
        _options = options.Value;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for the application to be fully started before processing
        // This ensures all dependencies are initialized
        await WaitForApplicationStartedAsync(stoppingToken);

        _logger.LogInformation(
            "InferenceQueueProcessorService started. ConfidenceThreshold={ConfidenceThreshold}",
            _options.ConfidenceThreshold);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Wait for an item to be enqueued
                var itemId = await _queue.DequeueAsync(stoppingToken);
                
                // Process the item
                await ProcessItemAsync(itemId, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when stopping
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing item from inference queue");
                
                // Continue processing other items even if one fails
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        _logger.LogInformation("InferenceQueueProcessorService stopped");
    }

    private async Task ProcessItemAsync(string itemId, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
        var inferenceService = scope.ServiceProvider.GetRequiredService<ITypeInferenceService>();
        var projectionWriter = scope.ServiceProvider.GetService<IProjectionWriter>();

        try
        {
            // Load the item from MongoDB
            var item = await itemRepository.GetByIdAsync(itemId, cancellationToken);
            if (item is null)
            {
                _logger.LogWarning("Item {ItemId} not found in MongoDB for inference processing", itemId);
                return;
            }

            // Run type inference
            var result = await inferenceService.InferAsync(item.Text, cancellationToken);

            // Update the item with inference results
            var existingConfidence = item.TypeConfidence ?? 0;
            if (result.Confidence > existingConfidence)
            {
                _logger.LogInformation(
                    "Processing item {ItemId}: type={Type}, confidence={OldConfidence}->{NewConfidence}",
                    item.Id,
                    result.InferredType,
                    existingConfidence,
                    result.Confidence);

                item.InferredType = result.InferredType;
                item.TypeConfidence = result.Confidence;

                // Auto-review if confidence is high enough
                if (result.Confidence >= _options.ConfidenceThreshold)
                {
                    item.LastReviewedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    _logger.LogDebug("Auto-reviewed item {ItemId} due to high confidence", item.Id);
                }

                // Update in MongoDB
                await itemRepository.UpdateAsync(item.Id, item, cancellationToken);

                // Sync to Redis projection (best effort, non-blocking)
                if (projectionWriter is not null)
                {
                    await projectionWriter.SyncItemAsync(item, cancellationToken);
                }
            }
            else
            {
                _logger.LogDebug(
                    "Skipping item {ItemId}: new confidence {NewConfidence} not higher than existing {ExistingConfidence}",
                    item.Id,
                    result.Confidence,
                    existingConfidence);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process item {ItemId}", itemId);
            // Don't rethrow - continue processing other items
        }
    }

    private async Task WaitForApplicationStartedAsync(CancellationToken stoppingToken)
    {
        var applicationStartedSource = new TaskCompletionSource();
        
        using var registration = _applicationLifetime.ApplicationStarted.Register(() =>
        {
            applicationStartedSource.TrySetResult();
        });

        // Wait for the application to be fully started
        await applicationStartedSource.Task.WaitAsync(stoppingToken);
    }
}
