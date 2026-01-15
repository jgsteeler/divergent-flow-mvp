namespace DivergentFlow.Application.Configuration;

/// <summary>
/// Configuration options for feature flags.
/// </summary>
public sealed class FeatureFlagOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = "FeatureFlags";
    
    /// <summary>
    /// Gets or sets whether the unified entity system is enabled.
    /// When false, the system uses the original Item/Collection/Capture entities.
    /// When true, the system uses the new unified Entity/EntityType system.
    /// </summary>
    public bool UnifiedEntitySystem { get; set; }
}
