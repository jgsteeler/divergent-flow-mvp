namespace DivergentFlow.Application.Models;

/// <summary>
/// Request model for creating a new capture.
/// Validation: Text must not be empty. TypeConfidence, if provided, must be between 0 and 100.
/// </summary>
public sealed class CreateCaptureRequest
{
    /// <summary>
    /// Gets or sets the text content of the capture.
    /// This field is required and cannot be empty.
    /// </summary>
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the inferred type of the capture (e.g., "action", "note", "question").
    /// This field is optional.
    /// </summary>
    public string? InferredType { get; set; }
    
    /// <summary>
    /// Gets or sets the confidence level (0-100) of the type inference.
    /// Must be between 0 and 100 if provided.
    /// </summary>
    public double? TypeConfidence { get; set; }
}
