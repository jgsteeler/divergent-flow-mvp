namespace DivergentFlow.Application.Models;

/// <summary>
/// Data transfer object representing a captured item with its metadata and type inference results.
/// </summary>
public sealed class CaptureDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the capture.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the captured text content.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Unix timestamp (milliseconds) when the capture was created.
    /// </summary>
    public long CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the Unix timestamp (milliseconds) when the capture was last updated.
    /// </summary>
    public long? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the inferred type classification, if available.
    /// </summary>
    public string? InferredType { get; set; }

    /// <summary>
    /// Gets or sets the confidence score (0-100) for the inferred type, if available.
    /// </summary>
    public double? TypeConfidence { get; set; }

    /// <summary>
    /// Gets or sets whether the capture has been migrated/processed to its final state.
    /// </summary>
    public bool IsMigrated { get; set; }
}
