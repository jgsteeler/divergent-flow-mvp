using DivergentFlow.Services.Models;
using DivergentFlow.Services.Repositories;
using Xunit;

namespace DivergentFlow.Services.Tests.Repositories;

/// <summary>
/// Unit tests for InMemoryCaptureRepository
/// </summary>
public class InMemoryCaptureRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoCaptures()
    {
        // Arrange
        var repository = new InMemoryCaptureRepository();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SaveAsync_SavesCapture_AndReturnsIt()
    {
        // Arrange
        var repository = new InMemoryCaptureRepository();
        var capture = new CaptureDto
        {
            Id = "test-id",
            Text = "Test capture",
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Act
        var result = await repository.SaveAsync(capture);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(capture.Id, result.Id);
        Assert.Equal(capture.Text, result.Text);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCapture_WhenExists()
    {
        // Arrange
        var repository = new InMemoryCaptureRepository();
        var capture = new CaptureDto
        {
            Id = "test-id",
            Text = "Test capture",
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        await repository.SaveAsync(capture);

        // Act
        var result = await repository.GetByIdAsync("test-id");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-id", result.Id);
        Assert.Equal("Test capture", result.Text);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var repository = new InMemoryCaptureRepository();

        // Act
        var result = await repository.GetByIdAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesCapture_WhenExists()
    {
        // Arrange
        var repository = new InMemoryCaptureRepository();
        var capture = new CaptureDto
        {
            Id = "test-id",
            Text = "Original text",
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        await repository.SaveAsync(capture);

        var updatedCapture = new CaptureDto
        {
            Id = "test-id",
            Text = "Updated text",
            CreatedAt = capture.CreatedAt,
            InferredType = "note",
            TypeConfidence = 95.5
        };

        // Act
        var result = await repository.UpdateAsync(updatedCapture);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-id", result.Id);
        Assert.Equal("Updated text", result.Text);
        Assert.Equal("note", result.InferredType);
        Assert.Equal(95.5, result.TypeConfidence);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var repository = new InMemoryCaptureRepository();
        var capture = new CaptureDto
        {
            Id = "nonexistent",
            Text = "Test",
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Act
        var result = await repository.UpdateAsync(capture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_DeletesCapture_WhenExists()
    {
        // Arrange
        var repository = new InMemoryCaptureRepository();
        var capture = new CaptureDto
        {
            Id = "test-id",
            Text = "Test capture",
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        await repository.SaveAsync(capture);

        // Act
        var result = await repository.DeleteAsync("test-id");

        // Assert
        Assert.True(result);
        var retrievedCapture = await repository.GetByIdAsync("test-id");
        Assert.Null(retrievedCapture);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotExists()
    {
        // Arrange
        var repository = new InMemoryCaptureRepository();

        // Act
        var result = await repository.DeleteAsync("nonexistent");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCaptures()
    {
        // Arrange
        var repository = new InMemoryCaptureRepository();
        var capture1 = new CaptureDto
        {
            Id = "id1",
            Text = "Capture 1",
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        var capture2 = new CaptureDto
        {
            Id = "id2",
            Text = "Capture 2",
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        await repository.SaveAsync(capture1);
        await repository.SaveAsync(capture2);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Id == "id1");
        Assert.Contains(result, c => c.Id == "id2");
    }
}
