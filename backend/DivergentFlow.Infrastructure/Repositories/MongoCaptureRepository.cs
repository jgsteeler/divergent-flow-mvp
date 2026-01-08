using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Configuration;
using DivergentFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DivergentFlow.Infrastructure.Repositories;

/// <summary>
/// MongoDB implementation of <see cref="ICaptureRepository"/>.
///
/// Captures are stored in the same MongoDB collection as Items.
/// A capture is represented as an <see cref="Item"/> with <c>Type == "capture"</c>.
/// </summary>
public sealed class MongoCaptureRepository : ICaptureRepository
{
    private const string CaptureType = "capture";

    private readonly IMongoCollection<Item> _items;
    private readonly ILogger<MongoCaptureRepository> _logger;

    private static FilterDefinition<Item> BuildUserFilter(string userId)
    {
        var userIsLocal = string.Equals(userId, "local", StringComparison.Ordinal);
        return userIsLocal
            ? Builders<Item>.Filter.Or(
                Builders<Item>.Filter.Eq(i => i.UserId, userId),
                Builders<Item>.Filter.Eq(i => i.UserId, null))
            : Builders<Item>.Filter.Eq(i => i.UserId, userId);
    }

    public MongoCaptureRepository(
        IMongoDatabase database,
        IOptions<MongoDbSettings> settings,
        ILogger<MongoCaptureRepository> logger)
    {
        _logger = logger;
        var collectionName = settings.Value.ItemsCollectionName;
        _items = database.GetCollection<Item>(collectionName);

        _logger.LogInformation(
            "MongoCaptureRepository initialized using items collection: {CollectionName}",
            collectionName);
    }

    public async Task<IReadOnlyList<Capture>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Item>.Filter.And(
                BuildUserFilter(userId),
                Builders<Item>.Filter.Eq(i => i.Type, CaptureType));
            var items = await _items.Find(filter).ToListAsync(cancellationToken);

            return items.Select(ToCapture).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving captures from MongoDB");
            throw;
        }
    }

    public async Task<Capture?> GetByIdAsync(string userId, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Item>.Filter.And(
                BuildUserFilter(userId),
                Builders<Item>.Filter.Eq(i => i.Id, id),
                Builders<Item>.Filter.Eq(i => i.Type, CaptureType));

            var item = await _items.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return item is null ? null : ToCapture(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving capture {CaptureId} from MongoDB", id);
            throw;
        }
    }

    public async Task<Capture> CreateAsync(string userId, Capture capture, CancellationToken cancellationToken = default)
    {
        try
        {
            capture.UserId = userId;
            var item = new Item
            {
                Id = capture.Id,
                UserId = userId,
                Type = CaptureType,
                Text = capture.Text,
                CreatedAt = capture.CreatedAt,
                InferredType = capture.InferredType,
                TypeConfidence = capture.TypeConfidence,
                LastReviewedAt = null,
                CollectionId = null
            };

            await _items.InsertOneAsync(item, cancellationToken: cancellationToken);
            return ToCapture(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating capture {CaptureId} in MongoDB", capture.Id);
            throw;
        }
    }

    public async Task<Capture?> UpdateAsync(string userId, string id, Capture updated, CancellationToken cancellationToken = default)
    {
        try
        {
            // Load existing so we don't accidentally wipe fields only present on Item.
            var existing = await GetItemByIdAsync(userId, id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            existing.UserId = userId;
            existing.Text = updated.Text;
            existing.InferredType = updated.InferredType;
            existing.TypeConfidence = updated.TypeConfidence;
            // Migration is not modeled on Item yet; treat as not migrated.

            var filter = Builders<Item>.Filter.And(
                BuildUserFilter(userId),
                Builders<Item>.Filter.Eq(i => i.Id, id),
                Builders<Item>.Filter.Eq(i => i.Type, CaptureType));

            var result = await _items.ReplaceOneAsync(filter, existing, cancellationToken: cancellationToken);
            if (result.MatchedCount == 0)
            {
                return null;
            }

            return ToCapture(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating capture {CaptureId} in MongoDB", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string userId, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Item>.Filter.And(
                BuildUserFilter(userId),
                Builders<Item>.Filter.Eq(i => i.Id, id),
                Builders<Item>.Filter.Eq(i => i.Type, CaptureType));

            var result = await _items.DeleteOneAsync(filter, cancellationToken);
            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting capture {CaptureId} from MongoDB", id);
            throw;
        }
    }

    public async Task<IReadOnlyList<Capture>> GetCapturesNeedingReInferenceAsync(
        string userId,
        double confidenceThreshold,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = Builders<Item>.Filter.And(
                BuildUserFilter(userId),
                Builders<Item>.Filter.Eq(i => i.Type, CaptureType),
                Builders<Item>.Filter.Or(
                    Builders<Item>.Filter.Eq(i => i.TypeConfidence, null),
                    Builders<Item>.Filter.Lt(i => i.TypeConfidence, confidenceThreshold))
            );

            var items = await _items.Find(filter).ToListAsync(cancellationToken);
            return items.Select(ToCapture).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving captures needing re-inference from MongoDB");
            throw;
        }
    }

    private async Task<Item?> GetItemByIdAsync(string userId, string id, CancellationToken cancellationToken)
    {
        var filter = Builders<Item>.Filter.And(
            BuildUserFilter(userId),
            Builders<Item>.Filter.Eq(i => i.Id, id),
            Builders<Item>.Filter.Eq(i => i.Type, CaptureType));

        return await _items.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    private static Capture ToCapture(Item item)
        => new()
        {
            Id = item.Id,
            UserId = item.UserId,
            Text = item.Text,
            CreatedAt = item.CreatedAt,
            InferredType = item.InferredType,
            TypeConfidence = item.TypeConfidence,
            IsMigrated = false
        };
}
