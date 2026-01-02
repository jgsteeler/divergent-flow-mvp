namespace DivergentFlow.Domain.Entities;

/// <summary>
/// Represents a collection that groups related items together.
/// </summary>
public sealed class Collection
{
    /// <summary>
    /// Gets or sets the unique identifier for the collection.
    /// </summary>
    public required string Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the collection.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the Unix timestamp (milliseconds) when the collection was created.
    /// </summary>
    public required long CreatedAt { get; set; }
}
