using System.Text.Json;

namespace DivergentFlow.Domain.Entities;

/// <summary>
/// Universal entity model - the core of the unified entity system.
/// Everything in the system (items, collections, captures, dashboards, etc.) is an entity.
/// Based on the unified entity architecture described in docs/unified.md Part 1.
/// 
/// Key principles:
/// - Single data model across all functionality
/// - Extensibility through entity types (no schema migrations)
/// - Multi-tenant by design (tenantId everywhere)
/// - Version history built-in (audit trail)
/// - Permission model integrated (visibility + ownership)
/// </summary>
public sealed class Entity
{
    /// <summary>
    /// Gets or sets the unique identifier for this entity.
    /// Format: "entity:{type}:{uniqueId}" (e.g., "entity:capture:abc123")
    /// </summary>
    public required string Id { get; set; }
    
    /// <summary>
    /// Gets or sets the entity type identifier that defines the schema for this entity.
    /// Format: "entityType:{typename}" (e.g., "entityType:itemType:capture")
    /// </summary>
    public required string EntityTypeId { get; set; }
    
    /// <summary>
    /// Gets or sets the tenant identifier for multi-tenancy.
    /// Format: "tenant:{orgId}" (e.g., "tenant:acme")
    /// In single-user mode, this can be "tenant:default"
    /// </summary>
    public required string TenantId { get; set; }
    
    /// <summary>
    /// Gets or sets the owner user identifier.
    /// Format: "user:{userId}" (e.g., "user:john")
    /// </summary>
    public required string OwnerId { get; set; }
    
    /// <summary>
    /// Gets or sets the flexible attributes for this entity.
    /// Schema is defined by the EntityType. Stored as JSON for flexibility.
    /// </summary>
    public Dictionary<string, JsonElement> Attributes { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the metadata (timestamps, version, visibility).
    /// </summary>
    public required EntityMetadata Metadata { get; set; }
    
    /// <summary>
    /// Gets or sets the relationships to other entities.
    /// </summary>
    public EntityRelationships Relationships { get; set; } = new();
}
