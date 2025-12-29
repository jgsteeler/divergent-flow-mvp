namespace DivergentFlow.Services.Models;

/// <summary>
/// Result of type inference operation
/// </summary>
public class TypeInferenceResult
{
    /// <summary>
    /// The inferred type (e.g., action, note, reminder)
    /// </summary>
    public string InferredType { get; set; } = string.Empty;

    /// <summary>
    /// Confidence level for the inference (0-100)
    /// </summary>
    public double Confidence { get; set; }
}
