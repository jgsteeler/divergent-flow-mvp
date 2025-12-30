using DivergentFlow.Services.Domain;
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
        var captures = await _repository.GetAllAsync();
        return captures.Select(ToDto);
    }

    public async Task<CaptureDto?> GetByIdAsync(string id)
    {
        _logger.LogDebug("Getting capture {Id}", id);
        var capture = await _repository.GetByIdAsync(id);
        return capture is null ? null : ToDto(capture);
    }

    public async Task<CaptureDto> CreateAsync(CreateCaptureRequest request)
    {
        _logger.LogDebug("Creating new capture");
        
        var capture = new Capture
        {
            Id = Guid.NewGuid().ToString(),
            Text = request.Text,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            InferredType = request.InferredType,
            TypeConfidence = request.TypeConfidence
        };

        var created = await _repository.CreateAsync(capture);
        return ToDto(created);
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

        var updated = await _repository.UpdateAsync(id, existingCapture);
        return updated is null ? null : ToDto(updated);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        _logger.LogDebug("Deleting capture {Id}", id);
        return await _repository.DeleteAsync(id);
    }

    private static CaptureDto ToDto(Capture capture) => new()
    {
        Id = capture.Id,
        Text = capture.Text,
        CreatedAt = capture.CreatedAt,
        InferredType = capture.InferredType,
        TypeConfidence = capture.TypeConfidence
    };
}
