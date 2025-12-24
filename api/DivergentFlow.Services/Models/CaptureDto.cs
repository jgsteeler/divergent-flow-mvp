namespace DivergentFlow.Services.Models;

/// <summary>
/// Data Transfer Object for Capture items
/// Matches the frontend Capture interface
/// </summary>
public class CaptureDto
{
    /// <summary>
    /// Unique identifier for the capture
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The captured text content
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the capture was created (Unix milliseconds)
    /// </summary>
    public long CreatedAt { get; set; }

    /// <summary>
    /// The inferred type of the item (note, action, or reminder)
    /// </summary>
    public string? InferredType { get; set; }

    /// <summary>
    /// Confidence score for the type inference (0-100)
    /// </summary>
    public double? TypeConfidence { get; set; }
}
