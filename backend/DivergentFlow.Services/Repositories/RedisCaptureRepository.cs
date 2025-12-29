using DivergentFlow.Services.Domain;
using DivergentFlow.Services.Models;
using Microsoft.Extensions.Logging;
using Redis.OM;
using Redis.OM.Searching;

namespace DivergentFlow.Services.Repositories;

/// <summary>
/// Redis implementation of ICaptureRepository using Redis.OM
/// Connects to Upstash Redis for data persistence
/// </summary>
public class RedisCaptureRepository : ICaptureRepository
{
    private readonly IRedisCollection<CaptureEntity> _captures;
    private readonly ILogger<RedisCaptureRepository> _logger;

    public RedisCaptureRepository(
        RedisConnectionProvider provider,
        ILogger<RedisCaptureRepository> logger)
    {
        _logger = logger;
        
        // Get collection for CaptureEntity from the provider
        _captures = provider.RedisCollection<CaptureEntity>();
    }

    public async Task<IReadOnlyList<Capture>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all captures from Redis");
        
        try
        {
            var entities = await _captures.ToListAsync();
            return entities.Select(ToDomain).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all captures from Redis");
            throw;
        }
    }

    public async Task<Capture?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting capture {Id} from Redis", id);
        
        try
        {
            var entity = await _captures.FindByIdAsync(id);
            return entity is null ? null : ToDomain(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting capture {Id} from Redis", id);
            throw;
        }
    }

    public async Task<Capture> CreateAsync(Capture capture, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Saving new capture to Redis");
        
        try
        {
            var entity = FromDomain(capture);

            // Insert the entity - Redis.OM will generate ID if entity.Id is null
            var insertedId = await _captures.InsertAsync(entity);

            // Update the entity with the ID (either generated or existing)
            entity.Id = insertedId;

            _logger.LogInformation("Saved capture with ID {Id} to Redis", insertedId);

            return ToDomain(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving capture to Redis");
            throw;
        }
    }

    public async Task<Capture?> UpdateAsync(string id, Capture updated, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating capture {Id} in Redis", id);
        
        try
        {
            // Check if the entity exists
            var existingEntity = await _captures.FindByIdAsync(id);
            if (existingEntity == null)
            {
                _logger.LogWarning("Capture {Id} not found for update", id);
                return null;
            }
            
            // Update the entity
            var entity = FromDomain(updated);
            entity.Id = id;
            entity.CreatedAt = existingEntity.CreatedAt;
            await _captures.UpdateAsync(entity);
            
            _logger.LogInformation("Updated capture {Id} in Redis", id);
            
            return ToDomain(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating capture {Id} in Redis", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting capture {Id} from Redis", id);
        
        try
        {
            // Check if the entity exists
            var entity = await _captures.FindByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Capture {Id} not found for deletion", id);
                return false;
            }
            
            // Delete the entity
            await _captures.DeleteAsync(entity);
            
            _logger.LogInformation("Deleted capture {Id} from Redis", id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting capture {Id} from Redis", id);
            throw;
        }
    }

    private static Capture ToDomain(CaptureEntity entity) => new()
    {
        Id = entity.Id ?? string.Empty,
        Text = entity.Text,
        CreatedAt = entity.CreatedAt,
        InferredType = entity.InferredType,
        TypeConfidence = entity.TypeConfidence
    };

    private static CaptureEntity FromDomain(Capture capture) => new()
    {
        Id = string.IsNullOrEmpty(capture.Id) ? null : capture.Id,
        Text = capture.Text,
        CreatedAt = capture.CreatedAt,
        InferredType = capture.InferredType,
        TypeConfidence = capture.TypeConfidence
    };
}
