using DivergentFlow.Services.Models;

namespace DivergentFlow.Services.Services;

/// <summary>
/// Service interface for managing capture items
/// Implementation will be added in future phases
/// </summary>
public interface ICaptureService
{
    /// <summary>
    /// Get all captures
    /// </summary>
    Task<IEnumerable<CaptureDto>> GetAllAsync();

    /// <summary>
    /// Get a capture by ID
    /// </summary>
    Task<CaptureDto?> GetByIdAsync(string id);

    /// <summary>
    /// Create a new capture
    /// </summary>
    Task<CaptureDto> CreateAsync(CreateCaptureRequest request);

    /// <summary>
    /// Update an existing capture
    /// </summary>
    Task<CaptureDto?> UpdateAsync(string id, UpdateCaptureRequest request);

    /// <summary>
    /// Delete a capture
    /// </summary>
    Task<bool> DeleteAsync(string id);
}
