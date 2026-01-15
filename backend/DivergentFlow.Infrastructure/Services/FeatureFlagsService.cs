using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Configuration;
using Microsoft.Extensions.Options;

namespace DivergentFlow.Infrastructure.Services;

/// <summary>
/// Implementation of feature flag service using configuration.
/// </summary>
public sealed class FeatureFlagsService : IFeatureFlags
{
    private readonly FeatureFlagOptions _options;

    public FeatureFlagsService(IOptions<FeatureFlagOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc/>
    public bool IsUnifiedEntitySystemEnabled => _options.UnifiedEntitySystem;
}
