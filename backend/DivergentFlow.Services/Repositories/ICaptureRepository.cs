using DivergentFlow.Services.Models;

namespace DivergentFlow.Services.Repositories;

/// <summary>
/// Repository interface for managing capture items storage
/// Provides abstraction over the data persistence layer
/// </summary>
public interface ICaptureRepository
{
    /// <summary>
    /// Get all captures from storage
    /// </summary>
    Task<IEnumerable<CaptureDto>> GetAllAsync();

    /// <summary>
    /// Get a capture by ID from storage
    /// </summary>
    Task<CaptureDto?> GetByIdAsync(string id);

    /// <summary>
    /// Save a new capture to storage
    /// </summary>
    Task<CaptureDto> SaveAsync(CaptureDto capture);

    /// <summary>
    /// Update an existing capture in storage
    /// </summary>
    Task<CaptureDto?> UpdateAsync(CaptureDto capture);

    /// <summary>
    /// Delete a capture from storage
    /// </summary>
    Task<bool> DeleteAsync(string id);
}
