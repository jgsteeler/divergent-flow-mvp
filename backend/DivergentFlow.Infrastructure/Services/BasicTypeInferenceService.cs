using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Models;

namespace DivergentFlow.Infrastructure.Services;

/// <summary>
/// Provides a basic type inference implementation for MVP purposes.
/// This is a temporary implementation that returns fixed values and should be replaced
/// with a more sophisticated ML-based inference service in the future.
/// </summary>
public sealed class BasicTypeInferenceService : ITypeInferenceService
{
    /// <summary>
    /// Infers the type for the provided text by returning a fixed "action" type with 50% confidence.
    /// This is a placeholder implementation for MVP and should be replaced with actual inference logic.
    /// </summary>
    /// <param name="text">The raw text input to analyze for type inference.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the inference operation.</param>
    /// <returns>
    /// A <see cref="TypeInferenceResult"/> containing the inferred type ("action") and confidence (50.0).
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="text"/> is null or whitespace.</exception>
    public Task<TypeInferenceResult> InferAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be null or empty", nameof(text));
        }

        return Task.FromResult(new TypeInferenceResult
        {
            InferredType = "action",
            Confidence = 50.0
        });
    }

    /// <summary>
    /// Accepts a user-confirmed type selection. This is a placeholder implementation that does nothing.
    /// </summary>
    /// <param name="request">The confirmation request containing the original inference and the confirmed type.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the confirmation operation.</param>
    /// <returns>A completed task.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    public Task ConfirmAsync(TypeConfirmationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Task.CompletedTask;
    }
}
