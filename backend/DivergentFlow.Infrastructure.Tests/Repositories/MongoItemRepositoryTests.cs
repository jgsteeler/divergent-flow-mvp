using DivergentFlow.Application.Configuration;
using DivergentFlow.Domain.Entities;
using DivergentFlow.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace DivergentFlow.Infrastructure.Tests.Repositories;

public sealed class MongoItemRepositoryTests
{
    private readonly Mock<IMongoDatabase> _mockDatabase;
    private readonly Mock<IMongoCollection<Item>> _mockCollection;
    private readonly Mock<ILogger<MongoItemRepository>> _mockLogger;
    private readonly MongoDbSettings _settings;
    private readonly MongoItemRepository _repository;

    public MongoItemRepositoryTests()
    {
        _mockDatabase = new Mock<IMongoDatabase>();
        _mockCollection = new Mock<IMongoCollection<Item>>();
        _mockLogger = new Mock<ILogger<MongoItemRepository>>();
        
        _settings = new MongoDbSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "test_db",
            ItemsCollectionName = "test_items"
        };

        var mockOptions = new Mock<IOptions<MongoDbSettings>>();
        mockOptions.Setup(o => o.Value).Returns(_settings);

        _mockDatabase
            .Setup(db => db.GetCollection<Item>(_settings.ItemsCollectionName, null))
            .Returns(_mockCollection.Object);

        _repository = new MongoItemRepository(
            _mockDatabase.Object,
            mockOptions.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = "1", Type = "capture", Text = "Test 1", CreatedAt = 1000 },
            new Item { Id = "2", Type = "capture", Text = "Test 2", CreatedAt = 2000 }
        };

        var mockCursor = new Mock<IAsyncCursor<Item>>();
        mockCursor.Setup(c => c.Current).Returns(items);
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockCollection
            .Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Item>>(),
                It.IsAny<FindOptions<Item, Item>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, i => i.Id == "1");
        Assert.Contains(result, i => i.Id == "2");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsItem()
    {
        // Arrange
        var item = new Item { Id = "test-id", Type = "capture", Text = "Test", CreatedAt = 1000 };

        var mockCursor = new Mock<IAsyncCursor<Item>>();
        mockCursor.Setup(c => c.Current).Returns(new List<Item> { item });
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockCollection
            .Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Item>>(),
                It.IsAny<FindOptions<Item, Item>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _repository.GetByIdAsync("test-id");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-id", result.Id);
        Assert.Equal("Test", result.Text);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var mockCursor = new Mock<IAsyncCursor<Item>>();
        mockCursor.Setup(c => c.Current).Returns(new List<Item>());
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockCollection
            .Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Item>>(),
                It.IsAny<FindOptions<Item, Item>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _repository.GetByIdAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesItemSuccessfully()
    {
        // Arrange
        var item = new Item { Id = "new-id", Type = "capture", Text = "New Item", CreatedAt = 3000 };

        _mockCollection
            .Setup(c => c.InsertOneAsync(
                It.IsAny<Item>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _repository.CreateAsync(item);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(item.Id, result.Id);
        Assert.Equal(item.Text, result.Text);
        _mockCollection.Verify(
            c => c.InsertOneAsync(item, null, default),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesAndReturnsItem()
    {
        // Arrange
        var item = new Item { Id = "update-id", Type = "capture", Text = "Updated", CreatedAt = 4000 };

        var mockResult = new Mock<ReplaceOneResult>();
        mockResult.Setup(r => r.MatchedCount).Returns(1);

        _mockCollection
            .Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Item>>(),
                item,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        // Act
        var result = await _repository.UpdateAsync("update-id", item);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(item.Id, result.Id);
        Assert.Equal("Updated", result.Text);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var item = new Item { Id = "nonexistent", Type = "capture", Text = "Test", CreatedAt = 5000 };

        var mockResult = new Mock<ReplaceOneResult>();
        mockResult.Setup(r => r.MatchedCount).Returns(0);

        _mockCollection
            .Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Item>>(),
                item,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        // Act
        var result = await _repository.UpdateAsync("nonexistent", item);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var mockResult = new Mock<DeleteResult>();
        mockResult.Setup(r => r.DeletedCount).Returns(1);

        _mockCollection
            .Setup(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<Item>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        // Act
        var result = await _repository.DeleteAsync("delete-id");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var mockResult = new Mock<DeleteResult>();
        mockResult.Setup(r => r.DeletedCount).Returns(0);

        _mockCollection
            .Setup(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<Item>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        // Act
        var result = await _repository.DeleteAsync("nonexistent");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetItemsNeedingReInferenceAsync_ReturnsItemsWithLowConfidence()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = "1", Type = "capture", Text = "Test 1", CreatedAt = 1000, TypeConfidence = null },
            new Item { Id = "2", Type = "capture", Text = "Test 2", CreatedAt = 2000, TypeConfidence = 50 }
        };

        var mockCursor = new Mock<IAsyncCursor<Item>>();
        mockCursor.Setup(c => c.Current).Returns(items);
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        _mockCollection
            .Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Item>>(),
                It.IsAny<FindOptions<Item, Item>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _repository.GetItemsNeedingReInferenceAsync(95);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, i => i.Id == "1");
        Assert.Contains(result, i => i.Id == "2");
    }
}
