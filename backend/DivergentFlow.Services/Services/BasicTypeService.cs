using DivergentFlow.Services.Models;

namespace DivergentFlow.Services.Services;

/// <summary>
/// Basic implementation of ITypeService for MVP
/// Returns "action" type with 50% confidence for all inputs
/// This will be replaced with ML-based inference in future iterations
/// </summary>
public class BasicTypeService : ITypeService
{
    /// <summary>
    /// Infer the type of a captured text
    /// MVP implementation: always returns "action" with 50% confidence
    /// </summary>
    /// <param name="text">The captured text to analyze</param>
    /// <returns>The inferred type and confidence level</returns>
    /// <exception cref="ArgumentException">Thrown when text is null or empty</exception>
    public Task<TypeInferenceResult> InferAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be null or empty", nameof(text));
        }

        var result = new TypeInferenceResult
        {
            InferredType = "action",
            Confidence = 50.0
        };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Confirm the type of a capture for learning purposes
    /// MVP implementation: no-op, doesn't store anything yet
    /// </summary>
    /// <param name="request">The confirmation details</param>
    /// <returns>Task representing the async operation</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public Task ConfirmAsync(TypeConfirmationRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // MVP: No-op implementation - doesn't store anything yet
        // Future: This will store learning data for improving inference
        return Task.CompletedTask;
    }
}
