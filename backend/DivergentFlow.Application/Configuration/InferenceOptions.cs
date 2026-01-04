namespace DivergentFlow.Application.Configuration;

/// <summary>
/// Configuration options for inference workflows (background processing and auto-review thresholds).
/// </summary>
public sealed class InferenceOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Inference";

    /// <summary>
    /// Gets or sets the confidence threshold below which items will be re-inferred and above which
    /// inference results may be auto-applied (e.g., auto-review).
    /// Range: 0-100 (e.g., 95 = 95%). Default is 95.
    /// </summary>
    public double ConfidenceThreshold { get; set; } = 95;

    /// <summary>
    /// Gets or sets the interval in seconds between background inference runs.
    /// Default is 60 seconds (1 minute).
    /// </summary>
    public int ProcessingIntervalSeconds { get; set; } = 60;
}
