using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Application.Abstractions;

/// <summary>
/// Defines the contract for persisting and retrieving <see cref="Collection"/> entities.
/// </summary>
public interface ICollectionRepository
{
    /// <summary>
    /// Retrieves all <see cref="Collection"/> entries.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only list containing all collections.
    /// </returns>
    Task<IReadOnlyList<Collection>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single <see cref="Collection"/> by its identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the collection to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The matching collection if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Collection?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new <see cref="Collection"/> entry.
    /// </summary>
    /// <param name="collection">
    /// The collection instance to create.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The created collection, including any values assigned by the underlying store.
    /// </returns>
    Task<Collection> CreateAsync(Collection collection, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing <see cref="Collection"/> entry.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the collection to update.
    /// </param>
    /// <param name="updated">
    /// The collection data to apply to the existing entry.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The updated collection if the entry exists; otherwise, <c>null</c>.
    /// </returns>
    Task<Collection?> UpdateAsync(string id, Collection updated, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a <see cref="Collection"/> entry by its identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the collection to delete.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// <c>true</c> if a collection was deleted; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
