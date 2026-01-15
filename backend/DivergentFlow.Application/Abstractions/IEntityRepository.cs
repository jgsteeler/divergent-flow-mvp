using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Application.Abstractions;

/// <summary>
/// Repository interface for Entity operations.
/// Part of the unified entity system described in docs/unified.md
/// </summary>
public interface IEntityRepository
{
    /// <summary>
    /// Creates a new entity.
    /// </summary>
    Task<Entity> CreateAsync(Entity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets an entity by its ID.
    /// </summary>
    Task<Entity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets entities by tenant ID.
    /// </summary>
    Task<List<Entity>> GetByTenantAsync(string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets entities by entity type ID.
    /// </summary>
    Task<List<Entity>> GetByEntityTypeAsync(string entityTypeId, string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets entities by owner ID.
    /// </summary>
    Task<List<Entity>> GetByOwnerAsync(string ownerId, string tenantId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    Task<Entity> UpdateAsync(Entity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an entity by its ID.
    /// </summary>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
