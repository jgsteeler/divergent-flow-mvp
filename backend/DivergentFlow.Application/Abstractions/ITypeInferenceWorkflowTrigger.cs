namespace DivergentFlow.Application.Abstractions;

/// <summary>
/// Defines the contract for triggering background type inference processing.
/// </summary>
public interface ITypeInferenceWorkflowTrigger
{
    /// <summary>
    /// Signals that a new capture has been created and should be queued for inference processing.
    /// This is a fire-and-forget operation that does not block.
    /// </summary>
    /// <param name="captureId">The ID of the newly created capture.</param>
    void TriggerInferenceWorkflow(string captureId);
}
