namespace DivergentFlow.Application.Models;

public sealed class TypeConfirmationRequest
{
    public string Text { get; set; } = string.Empty;
    public string InferredType { get; set; } = string.Empty;
    public double InferredConfidence { get; set; }
    public string ConfirmedType { get; set; } = string.Empty;
}
