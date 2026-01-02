namespace DivergentFlow.Application.Models;

/// <summary>
/// Request model for updating an existing item.
/// </summary>
public sealed class UpdateItemRequest
{
    /// <summary>
    /// Gets or sets the updated text content of the item.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional updated inferred type.
    /// </summary>
    public string? InferredType { get; set; }

    /// <summary>
    /// Gets or sets the optional updated confidence score.
    /// </summary>
    public double? TypeConfidence { get; set; }

    /// <summary>
    /// Gets or sets the optional collection ID this item belongs to.
    /// </summary>
    public string? CollectionId { get; set; }
}
