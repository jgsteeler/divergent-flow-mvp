namespace DivergentFlow.Application.Models;

/// <summary>
/// Represents the result of a type inference operation for captured text.
/// </summary>
public sealed class TypeInferenceResult
{
    /// <summary>
    /// Gets or sets the inferred type classification for the text input.
    /// </summary>
    public string InferredType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the confidence score (0-100) for the inferred type.
    /// </summary>
    public double Confidence { get; set; }
}
