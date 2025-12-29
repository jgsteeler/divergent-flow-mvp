using DivergentFlow.Services.Models;
using DivergentFlow.Services.Repositories;
using Microsoft.Extensions.Logging;

namespace DivergentFlow.Services.Services;

/// <summary>
/// Service implementation that uses ICaptureRepository for data persistence
/// Handles business logic and delegates storage operations to the repository
/// </summary>
public class CaptureService : ICaptureService
{
    private readonly ICaptureRepository _repository;
    private readonly ILogger<CaptureService> _logger;

    public CaptureService(
        ICaptureRepository repository,
        ILogger<CaptureService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<CaptureDto>> GetAllAsync()
    {
        _logger.LogDebug("Getting all captures");
        return await _repository.GetAllAsync();
    }

    public async Task<CaptureDto?> GetByIdAsync(string id)
    {
        _logger.LogDebug("Getting capture {Id}", id);
        return await _repository.GetByIdAsync(id);
    }

    public async Task<CaptureDto> CreateAsync(CreateCaptureRequest request)
    {
        _logger.LogDebug("Creating new capture");
        
        var capture = new CaptureDto
        {
            Id = Guid.NewGuid().ToString(),
            Text = request.Text,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            InferredType = request.InferredType,
            TypeConfidence = request.TypeConfidence
        };

        return await _repository.SaveAsync(capture);
    }

    public async Task<CaptureDto?> UpdateAsync(string id, UpdateCaptureRequest request)
    {
        _logger.LogDebug("Updating capture {Id}", id);
        
        // Get existing capture
        var existingCapture = await _repository.GetByIdAsync(id);
        if (existingCapture == null)
        {
            _logger.LogWarning("Capture {Id} not found for update", id);
            return null;
        }

        // Update properties
        existingCapture.Text = request.Text;
        existingCapture.InferredType = request.InferredType;
        existingCapture.TypeConfidence = request.TypeConfidence;

        return await _repository.UpdateAsync(existingCapture);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        _logger.LogDebug("Deleting capture {Id}", id);
        return await _repository.DeleteAsync(id);
    }
}
