using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DivergentFlow.Application.Services;

/// <summary>
/// Background service that periodically re-infers types for captures with low confidence scores.
/// This implements an eventual consistency model for type inference.
/// </summary>
public sealed class BackgroundTypeInferenceService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundTypeInferenceService> _logger;
    private readonly InferenceOptions _options;

    public BackgroundTypeInferenceService(
        IServiceProvider serviceProvider,
        ILogger<BackgroundTypeInferenceService> logger,
        IOptions<InferenceOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "BackgroundTypeInferenceService started. ConfidenceThreshold={ConfidenceThreshold}, IntervalSeconds={IntervalSeconds}",
            _options.ConfidenceThreshold,
            _options.ProcessingIntervalSeconds);

        // Wait a short delay on startup to allow the application to fully initialize
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessCapturesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing captures for re-inference");
            }

            // Wait for the configured interval before the next run
            await Task.Delay(
                TimeSpan.FromSeconds(_options.ProcessingIntervalSeconds),
                stoppingToken);
        }

        _logger.LogInformation("BackgroundTypeInferenceService stopped");
    }

    private async Task ProcessCapturesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICaptureRepository>();
        var inferenceService = scope.ServiceProvider.GetRequiredService<ITypeInferenceService>();

        var userContext = scope.ServiceProvider.GetService<IUserContext>();
        var userId = userContext?.UserId ?? "local";

        var capturesNeedingInference = await repository.GetCapturesNeedingReInferenceAsync(
            userId,
            _options.ConfidenceThreshold,
            cancellationToken);

        if (capturesNeedingInference.Count == 0)
        {
            _logger.LogDebug("No captures need re-inference at this time");
            return;
        }

        _logger.LogInformation(
            "Processing {Count} captures for re-inference",
            capturesNeedingInference.Count);

        var updatedCount = 0;
        var errorCount = 0;

        foreach (var capture in capturesNeedingInference)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                // Call the inference service
                var result = await inferenceService.InferAsync(capture.Text, cancellationToken);

                // Only update if the new confidence is higher than the existing one
                var existingConfidence = capture.TypeConfidence ?? 0;
                if (result.Confidence > existingConfidence)
                {
                    _logger.LogDebug(
                        "Updating capture {CaptureId}: type={Type}, confidence={OldConfidence}->{NewConfidence}",
                        capture.Id,
                        result.InferredType,
                        existingConfidence,
                        result.Confidence);

                    capture.InferredType = result.InferredType;
                    capture.TypeConfidence = result.Confidence;
                    capture.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    await repository.UpdateAsync(userId, capture.Id, capture, cancellationToken);
                    updatedCount++;
                }
                else
                {
                    _logger.LogDebug(
                        "Skipping capture {CaptureId}: new confidence {NewConfidence} not higher than existing {ExistingConfidence}",
                        capture.Id,
                        result.Confidence,
                        existingConfidence);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to re-infer type for capture {CaptureId}",
                    capture.Id);
                errorCount++;
            }
        }

        _logger.LogInformation(
            "Completed re-inference batch: {UpdatedCount} updated, {ErrorCount} errors, {TotalCount} processed",
            updatedCount,
            errorCount,
            capturesNeedingInference.Count);
    }
}
