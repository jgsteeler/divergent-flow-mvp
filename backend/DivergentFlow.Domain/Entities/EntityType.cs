namespace DivergentFlow.Domain.Entities;

/// <summary>
/// Defines a type of entity with its schema and behavior.
/// Entity types are the templates that define what attributes an entity can have.
/// Part of the unified entity system described in docs/unified.md
/// 
/// Examples of entity types:
/// - itemType:capture - For captured thoughts/ideas
/// - itemType:action - For actionable tasks
/// - itemType:note - For reference notes
/// - collectionType:project - For project collections
/// - dashboardType:adhd_focus - For ADHD-optimized dashboards
/// </summary>
public sealed class EntityType
{
    /// <summary>
    /// Gets or sets the unique identifier for this entity type.
    /// Format: "entityType:{typename}" (e.g., "entityType:itemType:capture")
    /// </summary>
    public required string Id { get; set; }
    
    /// <summary>
    /// Gets or sets the display name of this entity type.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the parent entity type ID for inheritance.
    /// Example: "entityType:item" might be the parent of "entityType:itemType:capture"
    /// </summary>
    public string? ParentTypeId { get; set; }
    
    /// <summary>
    /// Gets or sets the tenant ID. System entity types have "tenant:system".
    /// Custom entity types have the organization's tenant ID.
    /// </summary>
    public required string TenantId { get; set; }
    
    /// <summary>
    /// Gets or sets the description of this entity type.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the attribute definitions for this entity type.
    /// Defines the schema and validation rules for entities of this type.
    /// </summary>
    public List<AttributeDefinition> Attributes { get; set; } = new();
    
    /// <summary>
    /// Gets or sets whether this entity type is active.
    /// Inactive types cannot be used to create new entities.
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets when this entity type was created (Unix timestamp in milliseconds).
    /// </summary>
    public long CreatedDate { get; set; }
    
    /// <summary>
    /// Gets or sets when this entity type was last updated (Unix timestamp in milliseconds).
    /// </summary>
    public long UpdatedDate { get; set; }
}
