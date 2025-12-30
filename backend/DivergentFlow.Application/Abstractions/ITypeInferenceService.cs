using DivergentFlow.Application.Models;

namespace DivergentFlow.Application.Abstractions;

/// <summary>
/// Defines the contract for inferring item types from free-form text and
/// recording user confirmations for previously inferred types.
/// </summary>
public interface ITypeInferenceService
{
    /// <summary>
    /// Infers the most appropriate type and related metadata for the provided text.
    /// </summary>
    /// <param name="text">The raw text input to analyze for type inference.</param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the inference operation.
    /// </param>
    /// <returns>
    /// A <see cref="TypeInferenceResult"/> containing the inferred type and
    /// any associated confidence or metadata.
    /// </returns>
    Task<TypeInferenceResult> InferAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists or processes a user-confirmed type selection for a given inference.
    /// </summary>
    /// <param name="request">
    /// The confirmation request containing the original inference and the confirmed type.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the confirmation operation.
    /// </param>
    Task ConfirmAsync(TypeConfirmationRequest request, CancellationToken cancellationToken = default);
}
