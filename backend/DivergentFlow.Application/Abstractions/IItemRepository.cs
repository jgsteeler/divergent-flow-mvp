using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Application.Abstractions;

/// <summary>
/// Defines the contract for persisting and retrieving <see cref="Item"/> entities.
/// </summary>
public interface IItemRepository
{
    /// <summary>
    /// Retrieves all <see cref="Item"/> entries.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only list containing all items.
    /// </returns>
    Task<IReadOnlyList<Item>> GetAllAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single <see cref="Item"/> by its identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the item to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The matching item if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Item?> GetByIdAsync(string userId, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new <see cref="Item"/> entry.
    /// </summary>
    /// <param name="item">
    /// The item instance to create.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The created item, including any values assigned by the underlying store.
    /// </returns>
    Task<Item> CreateAsync(string userId, Item item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing <see cref="Item"/> entry.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the item to update.
    /// </param>
    /// <param name="updated">
    /// The item data to apply to the existing entry.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The updated item if the entry exists; otherwise, <c>null</c>.
    /// </returns>
    Task<Item?> UpdateAsync(string userId, string id, Item updated, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a <see cref="Item"/> entry by its identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the item to delete.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// <c>true</c> if an item was deleted; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> DeleteAsync(string userId, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all <see cref="Item"/> entries that have a null type confidence
    /// or a type confidence below the specified threshold.
    /// </summary>
    /// <param name="confidenceThreshold">
    /// The confidence threshold (0-100, e.g., 95 for 95%). Items with confidence below this value will be included.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only list containing all items that need re-inference.
    /// </returns>
    Task<IReadOnlyList<Item>> GetItemsNeedingReInferenceAsync(
        string userId,
        double confidenceThreshold,
        CancellationToken cancellationToken = default);
}
