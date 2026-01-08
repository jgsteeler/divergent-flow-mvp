using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Configuration;
using DivergentFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DivergentFlow.Infrastructure.Repositories;

/// <summary>
/// MongoDB implementation of <see cref="ICollectionRepository"/>.
/// Provides CRUD operations for <see cref="Collection"/> entities stored in MongoDB.
/// </summary>
public sealed class MongoCollectionRepository : ICollectionRepository
{
    private readonly IMongoCollection<Collection> _collection;
    private readonly ILogger<MongoCollectionRepository> _logger;

    private static FilterDefinition<Collection> BuildUserFilter(string userId)
    {
        var userIsLocal = string.Equals(userId, "local", StringComparison.Ordinal);
        return userIsLocal
            ? Builders<Collection>.Filter.Or(
                Builders<Collection>.Filter.Eq(c => c.UserId, userId),
                Builders<Collection>.Filter.Eq(c => c.UserId, null))
            : Builders<Collection>.Filter.Eq(c => c.UserId, userId);
    }

    public MongoCollectionRepository(
        IMongoDatabase database,
        IOptions<MongoDbSettings> settings,
        ILogger<MongoCollectionRepository> logger)
    {
        _logger = logger;
        var collectionName = settings.Value.CollectionsCollectionName;
        _collection = database.GetCollection<Collection>(collectionName);
        
        _logger.LogInformation(
            "MongoCollectionRepository initialized with collection: {CollectionName}",
            collectionName);
    }

    public async Task<IReadOnlyList<Collection>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var collections = await _collection
                .Find(BuildUserFilter(userId))
                .ToListAsync(cancellationToken);
            
            _logger.LogDebug("Retrieved {Count} collections from MongoDB", collections.Count);
            return collections;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all collections from MongoDB");
            throw;
        }
    }

    public async Task<Collection?> GetByIdAsync(string userId, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Collection>.Filter.And(
                BuildUserFilter(userId),
                Builders<Collection>.Filter.Eq(c => c.Id, id));
            var collection = await _collection
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (collection is null)
            {
                _logger.LogDebug("Collection with ID {CollectionId} not found in MongoDB", id);
            }
            
            return collection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving collection {CollectionId} from MongoDB", id);
            throw;
        }
    }

    public async Task<Collection> CreateAsync(string userId, Collection collection, CancellationToken cancellationToken = default)
    {
        try
        {
            collection.UserId = userId;
            await _collection.InsertOneAsync(collection, cancellationToken: cancellationToken);
            
            _logger.LogInformation(
                "Created collection in MongoDB: ID={CollectionId}, Name={Name}",
                collection.Id,
                collection.Name);
            
            return collection;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating collection in MongoDB: ID={CollectionId}",
                collection.Id);
            throw;
        }
    }

    public async Task<Collection?> UpdateAsync(string userId, string id, Collection updated, CancellationToken cancellationToken = default)
    {
        try
        {
            updated.UserId = userId;
            var filter = Builders<Collection>.Filter.And(
                BuildUserFilter(userId),
                Builders<Collection>.Filter.Eq(c => c.Id, id));
            var result = await _collection.ReplaceOneAsync(
                filter,
                updated,
                cancellationToken: cancellationToken);
            
            if (result.MatchedCount == 0)
            {
                _logger.LogWarning("Collection with ID {CollectionId} not found for update in MongoDB", id);
                return null;
            }
            
            _logger.LogInformation(
                "Updated collection in MongoDB: ID={CollectionId}, Name={Name}",
                updated.Id,
                updated.Name);
            
            return updated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating collection {CollectionId} in MongoDB", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string userId, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Collection>.Filter.And(
                BuildUserFilter(userId),
                Builders<Collection>.Filter.Eq(c => c.Id, id));
            var result = await _collection.DeleteOneAsync(filter, cancellationToken);
            
            if (result.DeletedCount > 0)
            {
                _logger.LogInformation("Deleted collection from MongoDB: ID={CollectionId}", id);
                return true;
            }
            
            _logger.LogWarning("Collection with ID {CollectionId} not found for deletion in MongoDB", id);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting collection {CollectionId} from MongoDB", id);
            throw;
        }
    }
}
