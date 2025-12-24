using System.ComponentModel.DataAnnotations;

namespace DivergentFlow.Services.Models;

/// <summary>
/// Request model for updating an existing capture
/// </summary>
public class UpdateCaptureRequest
{
    /// <summary>
    /// The updated text content
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Optional inferred type of the item (e.g., note, action, reminder)
    /// </summary>
    public string? InferredType { get; set; }

    /// <summary>
    /// Optional confidence score for the type inference (0-100)
    /// </summary>
    [Range(0, 100)]
    public double? TypeConfidence { get; set; }
}
