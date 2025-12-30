using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Application.Abstractions;

/// <summary>
/// Defines the contract for persisting and retrieving <see cref="Capture"/> entities.
/// </summary>
public interface ICaptureRepository
{
    /// <summary>
    /// Retrieves all <see cref="Capture"/> entries.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only list containing all captures.
    /// </returns>
    Task<IReadOnlyList<Capture>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single <see cref="Capture"/> by its identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the capture to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The matching capture if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Capture?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new <see cref="Capture"/> entry.
    /// </summary>
    /// <param name="capture">
    /// The capture instance to create.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The created capture, including any values assigned by the underlying store.
    /// </returns>
    Task<Capture> CreateAsync(Capture capture, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing <see cref="Capture"/> entry.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the capture to update.
    /// </param>
    /// <param name="updated">
    /// The capture data to apply to the existing entry.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The updated capture if the entry exists; otherwise, <c>null</c>.
    /// </returns>
    Task<Capture?> UpdateAsync(string id, Capture updated, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a <see cref="Capture"/> entry by its identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the capture to delete.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// <c>true</c> if a capture was deleted; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
