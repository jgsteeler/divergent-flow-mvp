using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Configuration;
using DivergentFlow.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace DivergentFlow.Application.Tests;

/// <summary>
/// Tests for the feature flags service.
/// </summary>
public sealed class FeatureFlagsTests
{
    [Fact]
    public void FeatureFlagsService_ReturnsCorrectValue_WhenFeatureEnabled()
    {
        // Arrange
        var options = Options.Create(new FeatureFlagOptions
        {
            UnifiedEntitySystem = true
        });
        var service = new FeatureFlagsService(options);

        // Act
        var isEnabled = service.IsUnifiedEntitySystemEnabled;

        // Assert
        Assert.True(isEnabled);
    }

    [Fact]
    public void FeatureFlagsService_ReturnsCorrectValue_WhenFeatureDisabled()
    {
        // Arrange
        var options = Options.Create(new FeatureFlagOptions
        {
            UnifiedEntitySystem = false
        });
        var service = new FeatureFlagsService(options);

        // Act
        var isEnabled = service.IsUnifiedEntitySystemEnabled;

        // Assert
        Assert.False(isEnabled);
    }

    [Fact]
    public void FeatureFlagOptions_HasCorrectSectionName()
    {
        // Assert
        Assert.Equal("FeatureFlags", FeatureFlagOptions.SectionName);
    }

    [Fact]
    public void FeatureFlagOptions_DefaultsToFalse()
    {
        // Arrange & Act
        var options = new FeatureFlagOptions();

        // Assert
        Assert.False(options.UnifiedEntitySystem);
    }
}
