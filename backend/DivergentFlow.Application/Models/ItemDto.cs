namespace DivergentFlow.Application.Models;

/// <summary>
/// Data transfer object representing an item with its metadata and type inference results.
/// </summary>
public sealed class ItemDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the item.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the item. Defaults to "capture" for newly created items.
    /// </summary>
    public string Type { get; set; } = "capture";

    /// <summary>
    /// Gets or sets the item text content.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Unix timestamp (milliseconds) when the item was created.
    /// </summary>
    public long CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the inferred type classification, if available.
    /// </summary>
    public string? InferredType { get; set; }

    /// <summary>
    /// Gets or sets the confidence score (0-100) for the inferred type, if available.
    /// </summary>
    public double? TypeConfidence { get; set; }

    /// <summary>
    /// Gets or sets the Unix timestamp (milliseconds) when the item was last reviewed, if available.
    /// </summary>
    public long? LastReviewedAt { get; set; }

    /// <summary>
    /// Gets or sets the ID of the collection this item belongs to, if any.
    /// </summary>
    public string? CollectionId { get; set; }
}
