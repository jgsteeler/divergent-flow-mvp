namespace DivergentFlow.Application.Abstractions;

/// <summary>
/// Service for checking feature flag status.
/// </summary>
public interface IFeatureFlags
{
    /// <summary>
    /// Checks if the unified entity system is enabled.
    /// </summary>
    bool IsUnifiedEntitySystemEnabled { get; }
}
