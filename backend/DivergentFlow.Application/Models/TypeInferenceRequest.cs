namespace DivergentFlow.Application.Models;

/// <summary>
/// Request model for inferring the type of a capture.
/// Validation: Text must not be empty.
/// </summary>
public sealed class TypeInferenceRequest
{
    /// <summary>
    /// Gets or sets the text content to analyze for type inference.
    /// This field is required and cannot be empty.
    /// </summary>
    public string Text { get; set; } = string.Empty;
}
