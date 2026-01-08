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
    /// <param name="userId">The user identifier that owns the item.</param>
    /// <param name="itemId">The ID of the newly created/updated item.</param>
    void TriggerInferenceWorkflow(string userId, string itemId);
}
