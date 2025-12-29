namespace DivergentFlow.Application.Models;

public sealed class TypeInferenceResult
{
    public string InferredType { get; set; } = string.Empty;
    public double Confidence { get; set; }
}
