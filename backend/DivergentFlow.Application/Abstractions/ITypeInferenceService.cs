using DivergentFlow.Application.Models;

namespace DivergentFlow.Application.Abstractions;

public interface ITypeInferenceService
{
    Task<TypeInferenceResult> InferAsync(string text, CancellationToken cancellationToken = default);
    Task ConfirmAsync(TypeConfirmationRequest request, CancellationToken cancellationToken = default);
}
