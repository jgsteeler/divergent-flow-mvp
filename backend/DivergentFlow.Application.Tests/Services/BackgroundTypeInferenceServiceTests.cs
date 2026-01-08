using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Configuration;
using DivergentFlow.Application.Models;
using DivergentFlow.Domain.Entities;
using Xunit;

namespace DivergentFlow.Application.Tests.Services;

public sealed class BackgroundTypeInferenceServiceTests
{
    private const string UserId = "local";

    [Fact]
    public async Task GetCapturesNeedingReInference_FiltersCorrectly()
    {
        // Arrange
        var captures = new List<Capture>
        {
            new Capture { Id = "1", Text = "Test 1", CreatedAt = 1000, IsMigrated = false, TypeConfidence = null },
            new Capture { Id = "2", Text = "Test 2", CreatedAt = 1001, IsMigrated = false, TypeConfidence = 50 },
            new Capture { Id = "3", Text = "Test 3", CreatedAt = 1002, IsMigrated = false, TypeConfidence = 96 },
            new Capture { Id = "4", Text = "Test 4", CreatedAt = 1003, IsMigrated = true, TypeConfidence = 50 },
        };

        var repository = new FakeCaptureRepository(captures);

        // Act
        var result = await repository.GetCapturesNeedingReInferenceAsync(UserId, 95);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Id == "1"); // null confidence
        Assert.Contains(result, c => c.Id == "2"); // confidence < threshold
    }

    [Fact]
    public async Task GetCapturesNeedingReInference_ExcludesMigratedCaptures()
    {
        // Arrange
        var captures = new List<Capture>
        {
            new Capture { Id = "1", Text = "Test 1", CreatedAt = 1000, IsMigrated = true, TypeConfidence = null },
            new Capture { Id = "2", Text = "Test 2", CreatedAt = 1001, IsMigrated = true, TypeConfidence = 50 },
        };

        var repository = new FakeCaptureRepository(captures);

        // Act
        var result = await repository.GetCapturesNeedingReInferenceAsync(UserId, 95);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCapturesNeedingReInference_ExcludesHighConfidenceCaptures()
    {
        // Arrange
        var captures = new List<Capture>
        {
            new Capture { Id = "1", Text = "Test 1", CreatedAt = 1000, IsMigrated = false, TypeConfidence = 96 },
            new Capture { Id = "2", Text = "Test 2", CreatedAt = 1001, IsMigrated = false, TypeConfidence = 99 },
        };

        var repository = new FakeCaptureRepository(captures);

        // Act
        var result = await repository.GetCapturesNeedingReInferenceAsync(UserId, 95);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void InferenceOptions_HasCorrectDefaults()
    {
        // Arrange & Act
        var options = new InferenceOptions();

        // Assert
        Assert.Equal(95, options.ConfidenceThreshold);
        Assert.Equal(60, options.ProcessingIntervalSeconds);
    }

    private sealed class FakeCaptureRepository : ICaptureRepository
    {
        private readonly List<Capture> _captures;

        public FakeCaptureRepository(List<Capture> captures)
        {
            _captures = captures;
        }

        public Task<IReadOnlyList<Capture>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<Capture>>(_captures);
        }

        public Task<Capture?> GetByIdAsync(string userId, string id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_captures.FirstOrDefault(c => c.Id == id));
        }

        public Task<Capture> CreateAsync(string userId, Capture capture, CancellationToken cancellationToken = default)
        {
            capture.UserId = userId;
            _captures.Add(capture);
            return Task.FromResult(capture);
        }

        public Task<Capture?> UpdateAsync(string userId, string id, Capture updated, CancellationToken cancellationToken = default)
        {
            var existing = _captures.FirstOrDefault(c => c.Id == id);
            if (existing == null) return Task.FromResult<Capture?>(null);
            
            _captures.Remove(existing);
            _captures.Add(updated);
            return Task.FromResult<Capture?>(updated);
        }

        public Task<bool> DeleteAsync(string userId, string id, CancellationToken cancellationToken = default)
        {
            var existing = _captures.FirstOrDefault(c => c.Id == id);
            if (existing == null) return Task.FromResult(false);
            
            _captures.Remove(existing);
            return Task.FromResult(true);
        }

        public Task<IReadOnlyList<Capture>> GetCapturesNeedingReInferenceAsync(
            string userId,
            double confidenceThreshold,
            CancellationToken cancellationToken = default)
        {
            var filtered = _captures
                .Where(c => !c.IsMigrated && 
                           (c.TypeConfidence == null || c.TypeConfidence < confidenceThreshold))
                .ToList();
            return Task.FromResult<IReadOnlyList<Capture>>(filtered);
        }
    }
}
