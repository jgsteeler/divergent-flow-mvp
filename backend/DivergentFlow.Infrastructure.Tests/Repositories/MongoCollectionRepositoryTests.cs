using DivergentFlow.Application.Configuration;
using DivergentFlow.Domain.Entities;
using DivergentFlow.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace DivergentFlow.Infrastructure.Tests.Repositories;

public sealed class MongoCollectionRepositoryTests
{
    private const string UserId = "local";
    private readonly Mock<IMongoDatabase> _mockDatabase;
    private readonly Mock<IMongoCollection<Collection>> _mockCollection;
    private readonly Mock<ILogger<MongoCollectionRepository>> _mockLogger;
    private readonly MongoCollectionRepository _repository;

    public MongoCollectionRepositoryTests()
    {
        _mockDatabase = new Mock<IMongoDatabase>();
        _mockCollection = new Mock<IMongoCollection<Collection>>();
        _mockLogger = new Mock<ILogger<MongoCollectionRepository>>();
        
        var settings = new MongoDbSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "test_db",
            CollectionsCollectionName = "test_collections"
        };

        var mockOptions = new Mock<IOptions<MongoDbSettings>>();
        mockOptions.Setup(o => o.Value).Returns(settings);

        _mockDatabase
            .Setup(db => db.GetCollection<Collection>(settings.CollectionsCollectionName, null))
            .Returns(_mockCollection.Object);

        _repository = new MongoCollectionRepository(
            _mockDatabase.Object,
            mockOptions.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAsync_CreatesCollectionSuccessfully()
    {
        // Arrange
        var collection = new Collection { Id = "coll-1", Name = "Test Collection", CreatedAt = 1000 };

        _mockCollection
            .Setup(c => c.InsertOneAsync(
                It.IsAny<Collection>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _repository.CreateAsync(UserId, collection);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(collection.Id, result.Id);
        Assert.Equal(collection.Name, result.Name);
        Assert.Equal(UserId, result.UserId);
        _mockCollection.Verify(
            c => c.InsertOneAsync(collection, null, default),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var mockResult = new Mock<DeleteResult>();
        mockResult.Setup(r => r.DeletedCount).Returns(1);

        _mockCollection
            .Setup(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<Collection>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        // Act
        var result = await _repository.DeleteAsync(UserId, "coll-1");

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
                It.IsAny<FilterDefinition<Collection>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResult.Object);

        // Act
        var result = await _repository.DeleteAsync(UserId, "nonexistent");

        // Assert
        Assert.False(result);
    }
}
