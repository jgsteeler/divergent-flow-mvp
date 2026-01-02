namespace DivergentFlow.Application.Models;

/// <summary>
/// Request model for updating an existing collection.
/// </summary>
public sealed class UpdateCollectionRequest
{
    /// <summary>
    /// Gets or sets the updated name of the collection.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
