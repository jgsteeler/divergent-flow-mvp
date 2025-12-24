using System.ComponentModel.DataAnnotations;

namespace DivergentFlow.Services.Models;

/// <summary>
/// Request model for creating a new capture
/// </summary>
public class CreateCaptureRequest
{
    /// <summary>
    /// The text content to capture
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Text { get; set; } = string.Empty;
}
