using System.ComponentModel.DataAnnotations;

namespace DivergentFlow.Services.Models;

/// <summary>
/// Request model for confirming type inference
/// Used for learning and improving future inferences
/// </summary>
public class TypeConfirmationRequest
{
    /// <summary>
    /// The original captured text
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The type that was inferred
    /// </summary>
    [Required]
    public string InferredType { get; set; } = string.Empty;

    /// <summary>
    /// The confidence level of the inference
    /// </summary>
    [Range(0, 100)]
    public double InferredConfidence { get; set; }

    /// <summary>
    /// The type confirmed by the user (may differ from inferred)
    /// </summary>
    [Required]
    public string ConfirmedType { get; set; } = string.Empty;
}
