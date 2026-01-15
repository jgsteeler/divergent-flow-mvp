namespace DivergentFlow.Domain.Entities;

/// <summary>
/// Metadata for an entity including version tracking, timestamps, and visibility.
/// Part of the unified entity system described in docs/unified.md
/// </summary>
public sealed class EntityMetadata
{
    /// <summary>
    /// Gets or sets when this entity was created (Unix timestamp in milliseconds).
    /// </summary>
    public required long CreatedDate { get; set; }
    
    /// <summary>
    /// Gets or sets when this entity was last updated (Unix timestamp in milliseconds).
    /// </summary>
    public required long UpdatedDate { get; set; }
    
    /// <summary>
    /// Gets or sets the version number for optimistic concurrency and audit trail.
    /// </summary>
    public int Version { get; set; } = 1;
    
    /// <summary>
    /// Gets or sets the visibility/sharing level of this entity.
    /// </summary>
    public EntityVisibility Visibility { get; set; } = EntityVisibility.Private;
}

/// <summary>
/// Defines the visibility/sharing level of an entity.
/// </summary>
public enum EntityVisibility
{
    /// <summary>
    /// Visible only to the owner
    /// </summary>
    Private,
    
    /// <summary>
    /// Visible to team members
    /// </summary>
    Team,
    
    /// <summary>
    /// Visible to entire organization
    /// </summary>
    Organization
}
