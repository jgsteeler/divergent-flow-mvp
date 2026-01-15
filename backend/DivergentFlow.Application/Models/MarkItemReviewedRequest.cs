namespace DivergentFlow.Application.Models;

/// <summary>
/// Request model for marking an item as reviewed
/// </summary>
public sealed class MarkItemReviewedRequest
{
    /// <summary>
    /// Optional confirmed type (if user corrected the inference)
    /// </summary>
    public string? ConfirmedType { get; set; }
    
    /// <summary>
    /// Optional confirmed confidence (if user corrected the inference)
    /// </summary>
    public double? ConfirmedConfidence { get; set; }
}
