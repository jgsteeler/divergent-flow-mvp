using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Models;

namespace DivergentFlow.Infrastructure.Services;

public sealed class BasicTypeInferenceService : ITypeInferenceService
{
    public Task<TypeInferenceResult> InferAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be null or empty", nameof(text));
        }

        return Task.FromResult(new TypeInferenceResult
        {
            InferredType = "action",
            Confidence = 50.0
        });
    }

    public Task ConfirmAsync(TypeConfirmationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Task.CompletedTask;
    }
}
