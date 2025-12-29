using DivergentFlow.Services.Models;

namespace DivergentFlow.Services.Services;

/// <summary>
/// Service interface for type inference operations
/// Allows for different inference implementations (basic, ML-based, etc.)
/// </summary>
public interface ITypeService
{
    /// <summary>
    /// Infer the type of a captured text
    /// </summary>
    /// <param name="text">The captured text to analyze</param>
    /// <returns>The inferred type and confidence level</returns>
    Task<TypeInferenceResult> InferAsync(string text);

    /// <summary>
    /// Confirm the type of a capture for learning purposes
    /// This allows the system to improve future inferences
    /// </summary>
    /// <param name="request">The confirmation details</param>
    /// <returns>Task representing the async operation</returns>
    Task ConfirmAsync(TypeConfirmationRequest request);
}
