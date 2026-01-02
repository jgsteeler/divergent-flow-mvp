namespace DivergentFlow.Application.Models;

/// <summary>
/// Data transfer object representing a collection.
/// </summary>
public sealed class CollectionDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the collection.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the collection.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Unix timestamp (milliseconds) when the collection was created.
    /// </summary>
    public long CreatedAt { get; set; }
}
