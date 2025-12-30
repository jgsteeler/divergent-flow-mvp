namespace DivergentFlow.Domain.Entities;

/// <summary>
/// Represents a captured thought, idea, task, or note from a user.
/// </summary>
public sealed class Capture
{
    /// <summary>
    /// Gets or sets the unique identifier for the capture.
    /// </summary>
    public required string Id { get; set; }
    
    /// <summary>
    /// Gets or sets the text content of the capture.
    /// </summary>
    public required string Text { get; set; }
    
    /// <summary>
    /// Gets or sets the Unix timestamp (milliseconds) when the capture was created.
    /// </summary>
    public required long CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the inferred type of the capture (e.g., "action", "note", "question").
    /// </summary>
    public string? InferredType { get; set; }
    
    /// <summary>
    /// Gets or sets the confidence level (0-100) of the type inference.
    /// </summary>
    public double? TypeConfidence { get; set; }
}
