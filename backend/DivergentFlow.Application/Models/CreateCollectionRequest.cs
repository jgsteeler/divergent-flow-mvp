namespace DivergentFlow.Application.Models;

/// <summary>
/// Request model for creating a new collection.
/// </summary>
public sealed class CreateCollectionRequest
{
    /// <summary>
    /// Gets or sets the name of the collection to create.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
