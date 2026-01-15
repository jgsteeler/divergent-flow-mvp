using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Application.Abstractions;

/// <summary>
/// Repository interface for EntityType operations.
/// Part of the unified entity system described in docs/unified.md
/// </summary>
public interface IEntityTypeRepository
{
    /// <summary>
    /// Creates a new entity type.
    /// </summary>
    Task<EntityType> CreateAsync(EntityType entityType, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets an entity type by its ID.
    /// </summary>
    Task<EntityType?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all entity types for a tenant.
    /// Includes system entity types (tenantId = "tenant:system").
    /// </summary>
    Task<List<EntityType>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets active entity types for a tenant.
    /// </summary>
    Task<List<EntityType>> GetActiveAsync(string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing entity type.
    /// </summary>
    Task<EntityType> UpdateAsync(EntityType entityType, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an entity type by its ID.
    /// Note: Should only be allowed if no entities of this type exist.
    /// </summary>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
