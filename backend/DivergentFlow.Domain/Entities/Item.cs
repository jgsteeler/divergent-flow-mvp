namespace DivergentFlow.Domain.Entities;

/// <summary>
/// Represents an item in the system. Items are the foundational unit of user-created content.
/// The base item type is "capture", which can be inferred to other types like "note", "action", "reminder", etc.
/// </summary>
public sealed class Item
{
    /// <summary>
    /// Gets or sets the unique identifier for the item.
    /// </summary>
    public required string Id { get; set; }
    
    /// <summary>
    /// Gets or sets the type of the item. Defaults to "capture" for newly created items.
    /// </summary>
    public required string Type { get; set; }
    
    /// <summary>
    /// Gets or sets the text content of the item.
    /// </summary>
    public required string Text { get; set; }
    
    /// <summary>
    /// Gets or sets the Unix timestamp (milliseconds) when the item was created.
    /// </summary>
    public required long CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the inferred type of the item (e.g., "action", "note", "reminder").
    /// This is the result of type inference and may differ from the base Type.
    /// </summary>
    public string? InferredType { get; set; }
    
    /// <summary>
    /// Gets or sets the confidence level (0-100) of the type inference.
    /// </summary>
    public double? TypeConfidence { get; set; }
    
    /// <summary>
    /// Gets or sets the Unix timestamp (milliseconds) when the item was last reviewed by the user.
    /// Null if the item has never been reviewed.
    /// </summary>
    public long? LastReviewedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the collection this item belongs to, if any.
    /// </summary>
    public string? CollectionId { get; set; }
}
