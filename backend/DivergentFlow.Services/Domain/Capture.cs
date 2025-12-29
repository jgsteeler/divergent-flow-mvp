namespace DivergentFlow.Services.Domain;

public sealed class Capture
{
    public required string Id { get; set; }
    public required string Text { get; set; }
    public required long CreatedAt { get; set; }
    public string? InferredType { get; set; }
    public double? TypeConfidence { get; set; }
}
