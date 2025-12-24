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
}
