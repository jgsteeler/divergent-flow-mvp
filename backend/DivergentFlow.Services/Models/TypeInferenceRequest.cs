using System.ComponentModel.DataAnnotations;

namespace DivergentFlow.Services.Models;

/// <summary>
/// Request model for type inference
/// </summary>
public class TypeInferenceRequest
{
    /// <summary>
    /// The captured text to analyze
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Text { get; set; } = string.Empty;
}
