using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Captures.Commands;
using DivergentFlow.Application.Features.Captures.Handlers;
using DivergentFlow.Application.Features.Captures.Mapping;
using DivergentFlow.Application.Models;
using DivergentFlow.Domain.Entities;
using Xunit;

namespace DivergentFlow.Application.Tests.Features.Captures;

public sealed class UpdateCaptureHandlerTests
{
    private readonly IMapper _mapper;

    public UpdateCaptureHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CaptureMappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Handle_ExistingCapture_UpdatesAndReturnsDto()
    {
        // Arrange
        var existingCapture = new Capture
        {
            Id = "capture-1",
            Text = "Original text",
            CreatedAt = 1000,
            InferredType = "note",
            TypeConfidence = 80.0
        };

        var repository = new FakeCaptureRepository(existingCapture);
        var handler = new UpdateCaptureHandler(repository, _mapper);

        var command = new UpdateCaptureCommand(
            Id: "capture-1",
            Text: "Updated text",
            InferredType: "action",
            TypeConfidence: 90.0
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("capture-1", result.Id);
        Assert.Equal("Updated text", result.Text);
        Assert.Equal("action", result.InferredType);
        Assert.Equal(90.0, result.TypeConfidence);
        Assert.Equal(1000, result.CreatedAt); // Should preserve original CreatedAt
    }

    [Fact]
    public async Task Handle_NonExistingCapture_ReturnsNull()
    {
        // Arrange
        var repository = new FakeCaptureRepository(null);
        var handler = new UpdateCaptureHandler(repository, _mapper);

        var command = new UpdateCaptureCommand(
            Id: "non-existing",
            Text: "Updated text",
            InferredType: "action",
            TypeConfidence: 90.0
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_UpdateWithNullTypeAndConfidence_Works()
    {
        // Arrange
        var existingCapture = new Capture
        {
            Id = "capture-1",
            Text = "Original text",
            CreatedAt = 1000,
            InferredType = "note",
            TypeConfidence = 80.0
        };

        var repository = new FakeCaptureRepository(existingCapture);
        var handler = new UpdateCaptureHandler(repository, _mapper);

        var command = new UpdateCaptureCommand(
            Id: "capture-1",
            Text: "Updated text",
            InferredType: null,
            TypeConfidence: null
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated text", result.Text);
        Assert.Null(result.InferredType);
        Assert.Null(result.TypeConfidence);
    }

    [Fact]
    public async Task Handle_PreservesCreatedAtTimestamp()
    {
        // Arrange
        var originalCreatedAt = 1234567890;
        var existingCapture = new Capture
        {
            Id = "capture-1",
            Text = "Original text",
            CreatedAt = originalCreatedAt,
            InferredType = null,
            TypeConfidence = null
        };

        var repository = new FakeCaptureRepository(existingCapture);
        var handler = new UpdateCaptureHandler(repository, _mapper);

        var command = new UpdateCaptureCommand(
            Id: "capture-1",
            Text: "Updated text",
            InferredType: "action",
            TypeConfidence: 85.0
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(originalCreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task Handle_CancellationToken_PassedToRepository()
    {
        // Arrange
        var existingCapture = new Capture
        {
            Id = "capture-1",
            Text = "Original text",
            CreatedAt = 1000,
            InferredType = null,
            TypeConfidence = null
        };

        var repository = new CancellationAwareFakeRepository(existingCapture);
        var handler = new UpdateCaptureHandler(repository, _mapper);

        var command = new UpdateCaptureCommand(
            Id: "capture-1",
            Text: "Updated text",
            InferredType: "action",
            TypeConfidence: 85.0
        );

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => handler.Handle(command, cts.Token));
    }

    private sealed class FakeCaptureRepository : ICaptureRepository
    {
        private Capture? _capture;

        public FakeCaptureRepository(Capture? capture)
        {
            _capture = capture;
        }

        public Task<IReadOnlyList<Capture>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Capture?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_capture?.Id == id ? _capture : null);
        }

        public Task<Capture> CreateAsync(Capture capture, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Capture?> UpdateAsync(string id, Capture updated, CancellationToken cancellationToken = default)
        {
            if (_capture?.Id != id)
            {
                return Task.FromResult<Capture?>(null);
            }

            // Simulate the repository behavior - preserve CreatedAt
            _capture = new Capture
            {
                Id = updated.Id,
                Text = updated.Text,
                CreatedAt = _capture.CreatedAt, // Preserve original
                InferredType = updated.InferredType,
                TypeConfidence = updated.TypeConfidence
            };

            return Task.FromResult<Capture?>(_capture);
        }

        public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Capture>> GetCapturesNeedingReInferenceAsync(
            double confidenceThreshold,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class CancellationAwareFakeRepository : ICaptureRepository
    {
        private readonly Capture? _capture;

        public CancellationAwareFakeRepository(Capture? capture)
        {
            _capture = capture;
        }

        public Task<IReadOnlyList<Capture>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Capture?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_capture?.Id == id ? _capture : null);
        }

        public Task<Capture> CreateAsync(Capture capture, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Capture?> UpdateAsync(string id, Capture updated, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Capture>> GetCapturesNeedingReInferenceAsync(
            double confidenceThreshold,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
