using DivergentFlow.Services.Models;
using DivergentFlow.Services.Repositories;
using DivergentFlow.Services.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DivergentFlow.Services.Tests.Services;

/// <summary>
/// Unit tests for CaptureService
/// </summary>
public class CaptureServiceTests
{
    private readonly Mock<ICaptureRepository> _mockRepository;
    private readonly Mock<ILogger<CaptureService>> _mockLogger;
    private readonly CaptureService _service;

    public CaptureServiceTests()
    {
        _mockRepository = new Mock<ICaptureRepository>();
        _mockLogger = new Mock<ILogger<CaptureService>>();
        _service = new CaptureService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_CallsRepository()
    {
        // Arrange
        var expectedCaptures = new List<CaptureDto>
        {
            new() { Id = "1", Text = "Test 1", CreatedAt = 1000 },
            new() { Id = "2", Text = "Test 2", CreatedAt = 2000 }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedCaptures);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(expectedCaptures, result);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_CallsRepository()
    {
        // Arrange
        var expectedCapture = new CaptureDto
        {
            Id = "test-id",
            Text = "Test capture",
            CreatedAt = 1000
        };
        _mockRepository.Setup(r => r.GetByIdAsync("test-id")).ReturnsAsync(expectedCapture);

        // Act
        var result = await _service.GetByIdAsync("test-id");

        // Assert
        Assert.Equal(expectedCapture, result);
        _mockRepository.Verify(r => r.GetByIdAsync("test-id"), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CreatesCapture_WithGeneratedId()
    {
        // Arrange
        var request = new CreateCaptureRequest
        {
            Text = "Test capture",
            InferredType = "note",
            TypeConfidence = 95.5
        };

        CaptureDto? savedCapture = null;
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<CaptureDto>()))
            .Callback<CaptureDto>(c => savedCapture = c)
            .ReturnsAsync((CaptureDto c) => c);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(savedCapture);
        Assert.False(string.IsNullOrEmpty(savedCapture.Id));
        Assert.Equal("Test capture", savedCapture.Text);
        Assert.Equal("note", savedCapture.InferredType);
        Assert.Equal(95.5, savedCapture.TypeConfidence);
        Assert.True(savedCapture.CreatedAt > 0);
        _mockRepository.Verify(r => r.SaveAsync(It.IsAny<CaptureDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingCapture()
    {
        // Arrange
        var existingCapture = new CaptureDto
        {
            Id = "test-id",
            Text = "Original text",
            CreatedAt = 1000
        };

        var updateRequest = new UpdateCaptureRequest
        {
            Text = "Updated text",
            InferredType = "action",
            TypeConfidence = 87.3
        };

        _mockRepository.Setup(r => r.GetByIdAsync("test-id")).ReturnsAsync(existingCapture);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<CaptureDto>()))
            .ReturnsAsync((CaptureDto c) => c);

        // Act
        var result = await _service.UpdateAsync("test-id", updateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-id", result.Id);
        Assert.Equal("Updated text", result.Text);
        Assert.Equal("action", result.InferredType);
        Assert.Equal(87.3, result.TypeConfidence);
        _mockRepository.Verify(r => r.GetByIdAsync("test-id"), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<CaptureDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenCaptureNotFound()
    {
        // Arrange
        var updateRequest = new UpdateCaptureRequest
        {
            Text = "Updated text"
        };

        _mockRepository.Setup(r => r.GetByIdAsync("nonexistent")).ReturnsAsync((CaptureDto?)null);

        // Act
        var result = await _service.UpdateAsync("nonexistent", updateRequest);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync("nonexistent"), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<CaptureDto>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepository()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync("test-id")).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync("test-id");

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync("test-id"), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenCaptureNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync("nonexistent")).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync("nonexistent");

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.DeleteAsync("nonexistent"), Times.Once);
    }
}
