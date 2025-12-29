using DivergentFlow.Services.Models;
using Microsoft.Extensions.Configuration;
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
    private readonly RedisConnectionProvider _provider;
    private readonly IRedisCollection<CaptureEntity> _captures;
    private readonly ILogger<RedisCaptureRepository> _logger;

    public RedisCaptureRepository(
        IConfiguration configuration,
        ILogger<RedisCaptureRepository> logger)
    {
        _logger = logger;
        
        // Get Redis connection string from environment variables
        var redisUrl = configuration["REDIS_URL"] 
            ?? throw new InvalidOperationException("REDIS_URL environment variable is not set");
        
        var redisToken = configuration["REDIS_TOKEN"] 
            ?? throw new InvalidOperationException("REDIS_TOKEN environment variable is not set");

        // Clean up URL - remove http:// or https:// if present
        redisUrl = redisUrl.Replace("http://", "").Replace("https://", "");

        // Build connection string with authentication
        // Upstash Redis format: redis://default:{token}@{host}:{port}
        var connectionString = $"redis://default:{redisToken}@{redisUrl}";
        
        _logger.LogInformation("Connecting to Redis at {RedisUrl}", redisUrl);
        
        // Initialize Redis connection provider
        _provider = new RedisConnectionProvider(connectionString);
        
        // Get collection for CaptureEntity
        _captures = _provider.RedisCollection<CaptureEntity>();
        
        // Create index if it doesn't exist
        try
        {
            _provider.Connection.CreateIndex(typeof(CaptureEntity));
            _logger.LogInformation("Redis index created or already exists for CaptureEntity");
        }
        catch (Exception ex)
        {
            // Index might already exist, which is fine
            _logger.LogDebug(ex, "Index creation attempt (may already exist)");
        }
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
            
            // Insert the entity and get the generated ID
            var insertedId = await _captures.InsertAsync(entity);
            
            // Update the entity with the generated ID
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
