namespace DivergentFlow.Application.Configuration;

/// <summary>
/// Configuration options for the type inference background service.
/// </summary>
public sealed class TypeInferenceOptions
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "TypeInference";

    /// <summary>
    /// Gets or sets the confidence threshold below which captures will be re-inferred.
    /// Range: 0-100 (e.g., 95 = 95%). Default is 95.
    /// </summary>
    public double ConfidenceThreshold { get; set; } = 95;

    /// <summary>
    /// Gets or sets the interval in seconds between background re-inference runs.
    /// Default is 60 seconds (1 minute).
    /// </summary>
    public int ProcessingIntervalSeconds { get; set; } = 60;
}
