using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Configuration;
using DivergentFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DivergentFlow.Infrastructure.Repositories;

/// <summary>
/// MongoDB implementation of <see cref="IItemRepository"/>.
/// Provides CRUD operations for <see cref="Item"/> entities stored in MongoDB.
/// </summary>
public sealed class MongoItemRepository : IItemRepository
{
    private readonly IMongoCollection<Item> _collection;
    private readonly ILogger<MongoItemRepository> _logger;

    public MongoItemRepository(
        IMongoDatabase database,
        IOptions<MongoDbSettings> settings,
        ILogger<MongoItemRepository> logger)
    {
        _logger = logger;
        var collectionName = settings.Value.ItemsCollectionName;
        _collection = database.GetCollection<Item>(collectionName);
        
        _logger.LogInformation(
            "MongoItemRepository initialized with collection: {CollectionName}",
            collectionName);
    }

    public async Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _collection
                .Find(FilterDefinition<Item>.Empty)
                .ToListAsync(cancellationToken);
            
            _logger.LogDebug("Retrieved {Count} items from MongoDB", items.Count);
            return items;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all items from MongoDB");
            throw;
        }
    }

    public async Task<Item?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Item>.Filter.Eq(i => i.Id, id);
            var item = await _collection
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (item is null)
            {
                _logger.LogDebug("Item with ID {ItemId} not found in MongoDB", id);
            }
            
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving item {ItemId} from MongoDB", id);
            throw;
        }
    }

    public async Task<Item> CreateAsync(Item item, CancellationToken cancellationToken = default)
    {
        try
        {
            await _collection.InsertOneAsync(item, cancellationToken: cancellationToken);
            
            _logger.LogInformation(
                "Created item in MongoDB: ID={ItemId}, Type={Type}",
                item.Id,
                item.Type);
            
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating item in MongoDB: ID={ItemId}",
                item.Id);
            throw;
        }
    }

    public async Task<Item?> UpdateAsync(string id, Item updated, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Item>.Filter.Eq(i => i.Id, id);
            var result = await _collection.ReplaceOneAsync(
                filter,
                updated,
                cancellationToken: cancellationToken);
            
            if (result.MatchedCount == 0)
            {
                _logger.LogWarning("Item with ID {ItemId} not found for update in MongoDB", id);
                return null;
            }
            
            _logger.LogInformation(
                "Updated item in MongoDB: ID={ItemId}, Type={Type}",
                updated.Id,
                updated.Type);
            
            return updated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating item {ItemId} in MongoDB", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Item>.Filter.Eq(i => i.Id, id);
            var result = await _collection.DeleteOneAsync(filter, cancellationToken);
            
            if (result.DeletedCount > 0)
            {
                _logger.LogInformation("Deleted item from MongoDB: ID={ItemId}", id);
                return true;
            }
            
            _logger.LogWarning("Item with ID {ItemId} not found for deletion in MongoDB", id);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item {ItemId} from MongoDB", id);
            throw;
        }
    }

    public async Task<IReadOnlyList<Item>> GetItemsNeedingReInferenceAsync(
        double confidenceThreshold,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Item>.Filter.Or(
                Builders<Item>.Filter.Eq(i => i.TypeConfidence, null),
                Builders<Item>.Filter.Lt(i => i.TypeConfidence, confidenceThreshold)
            );
            
            var items = await _collection
                .Find(filter)
                .ToListAsync(cancellationToken);
            
            _logger.LogDebug(
                "Retrieved {Count} items needing re-inference (threshold: {Threshold})",
                items.Count,
                confidenceThreshold);
            
            return items;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving items needing re-inference from MongoDB");
            throw;
        }
    }
}
