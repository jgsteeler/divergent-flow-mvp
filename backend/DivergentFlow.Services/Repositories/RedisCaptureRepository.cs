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

    public async Task<IEnumerable<CaptureDto>> GetAllAsync()
    {
        _logger.LogDebug("Getting all captures from Redis");
        
        try
        {
            var entities = await _captures.ToListAsync();
            return entities.Select(e => e.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all captures from Redis");
            throw;
        }
    }

    public async Task<CaptureDto?> GetByIdAsync(string id)
    {
        _logger.LogDebug("Getting capture {Id} from Redis", id);
        
        try
        {
            var entity = await _captures.FindByIdAsync(id);
            return entity?.ToDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting capture {Id} from Redis", id);
            throw;
        }
    }

    public async Task<CaptureDto> SaveAsync(CaptureDto capture)
    {
        _logger.LogDebug("Saving new capture to Redis");
        
        try
        {
            var entity = CaptureEntity.FromDto(capture);
            
            // Redis.OM will generate an ID if not provided
            // If capture.Id is empty, let Redis generate it
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = null; // Let Redis.OM generate
            }
            
            // Insert the entity
            var insertedId = await _captures.InsertAsync(entity);
            
            // Update the entity with the ID (either generated or existing)
            entity.Id = insertedId;
            
            _logger.LogInformation("Saved capture with ID {Id} to Redis", insertedId);
            
            return entity.ToDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving capture to Redis");
            throw;
        }
    }

    public async Task<CaptureDto?> UpdateAsync(CaptureDto capture)
    {
        _logger.LogDebug("Updating capture {Id} in Redis", capture.Id);
        
        try
        {
            // Check if the entity exists
            var existingEntity = await _captures.FindByIdAsync(capture.Id);
            if (existingEntity == null)
            {
                _logger.LogWarning("Capture {Id} not found for update", capture.Id);
                return null;
            }
            
            // Update the entity
            var entity = CaptureEntity.FromDto(capture);
            await _captures.UpdateAsync(entity);
            
            _logger.LogInformation("Updated capture {Id} in Redis", capture.Id);
            
            return entity.ToDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating capture {Id} in Redis", capture.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string id)
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
}
