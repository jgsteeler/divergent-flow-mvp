namespace DivergentFlow.Application.Models;

/// <summary>
/// Request model for creating a new item.
/// </summary>
public sealed class CreateItemRequest
{
    /// <summary>
    /// Gets or sets the text content of the item to create.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional inferred type for the item.
    /// </summary>
    public string? InferredType { get; set; }

    /// <summary>
    /// Gets or sets the optional confidence score for the inferred type.
    /// </summary>
    public double? TypeConfidence { get; set; }

    /// <summary>
    /// Gets or sets the optional collection ID this item belongs to.
    /// </summary>
    public string? CollectionId { get; set; }
}
