namespace DivergentFlow.Application.Models;

public sealed class CreateCaptureRequest
{
    public string Text { get; set; } = string.Empty;
    public string? InferredType { get; set; }
    public double? TypeConfidence { get; set; }
}
