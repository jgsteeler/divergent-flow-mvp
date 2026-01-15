namespace DivergentFlow.Domain.Entities;

/// <summary>
/// Relationships between entities including hierarchical and explicit links.
/// Part of the unified entity system described in docs/unified.md
/// </summary>
public sealed class EntityRelationships
{
    /// <summary>
    /// Gets or sets the list of parent entity IDs (hierarchical relationships).
    /// </summary>
    public List<string> ParentIds { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the list of entity IDs that reference this entity (reverse relationships for queries).
    /// </summary>
    public List<string> ReferencedByIds { get; set; } = new();
    
    /// <summary>
    /// Gets or sets explicit named relationships to other entities.
    /// Key is the relationship name (e.g., "assignedTo", "dependsOn", "relatedTo").
    /// Value is the entity ID.
    /// </summary>
    public Dictionary<string, string> LinkedEntities { get; set; } = new();
}
