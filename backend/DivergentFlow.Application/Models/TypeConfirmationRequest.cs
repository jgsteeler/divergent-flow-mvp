namespace DivergentFlow.Application.Models;

/// <summary>
/// Request model for confirming and correcting a type inference.
/// Validation: Text, InferredType, and ConfirmedType must not be empty. InferredConfidence must be between 0 and 100.
/// </summary>
public sealed class TypeConfirmationRequest
{
    /// <summary>
    /// Gets or sets the text content of the capture.
    /// This field is required and cannot be empty.
    /// </summary>
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the originally inferred type.
    /// This field is required and cannot be empty.
    /// </summary>
    public string InferredType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the confidence level (0-100) of the original inference.
    /// Must be between 0 and 100.
    /// </summary>
    public double InferredConfidence { get; set; }
    
    /// <summary>
    /// Gets or sets the user-confirmed type.
    /// This field is required and cannot be empty.
    /// </summary>
    public string ConfirmedType { get; set; } = string.Empty;
}
