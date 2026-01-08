namespace DivergentFlow.Domain.Entities;

/// <summary>
/// Represents a captured thought, idea, task, or note from a user.
/// </summary>
public sealed class Capture
{
    /// <summary>
    /// Gets or sets the user identifier that owns this capture.
    ///
    /// Note: This is currently optional to avoid breaking existing persisted documents.
    /// When user identity is fully implemented, this will become required.
    /// </summary>
    public string? UserId { get; set; }

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
    /// Gets or sets the Unix timestamp (milliseconds) when the capture was last updated.
    /// </summary>
    public long? UpdatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the inferred type of the capture (e.g., "action", "note", "question").
    /// </summary>
    public string? InferredType { get; set; }
    
    /// <summary>
    /// Gets or sets the confidence level (0-100) of the type inference.
    /// </summary>
    public double? TypeConfidence { get; set; }
    
    /// <summary>
    /// Gets or sets whether the capture has been migrated/processed to its final state.
    /// </summary>
    public bool IsMigrated { get; set; }
}
