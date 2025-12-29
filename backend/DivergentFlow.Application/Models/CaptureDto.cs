namespace DivergentFlow.Application.Models;

public sealed class CaptureDto
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public long CreatedAt { get; set; }
    public string? InferredType { get; set; }
    public double? TypeConfidence { get; set; }
}
