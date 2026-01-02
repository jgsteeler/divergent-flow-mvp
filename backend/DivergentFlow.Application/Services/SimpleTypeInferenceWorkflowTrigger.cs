using DivergentFlow.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace DivergentFlow.Application.Services;

/// <summary>
/// Simple implementation of the type inference workflow trigger that logs the event.
/// The actual processing is handled by the BackgroundTypeInferenceService periodically.
/// </summary>
public sealed class SimpleTypeInferenceWorkflowTrigger : ITypeInferenceWorkflowTrigger
{
    private readonly ILogger<SimpleTypeInferenceWorkflowTrigger> _logger;

    public SimpleTypeInferenceWorkflowTrigger(ILogger<SimpleTypeInferenceWorkflowTrigger> logger)
    {
        _logger = logger;
    }

    public void TriggerInferenceWorkflow(string captureId)
    {
        _logger.LogDebug(
            "Type inference workflow triggered for capture {CaptureId}. Will be processed by background service.",
            captureId);
    }
}
