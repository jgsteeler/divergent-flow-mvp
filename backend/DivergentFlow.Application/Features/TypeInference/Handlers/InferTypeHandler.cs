using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.TypeInference.Queries;
using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.TypeInference.Handlers;

public sealed class InferTypeHandler : IRequestHandler<InferTypeQuery, TypeInferenceResult>
{
    private readonly ITypeInferenceService _typeInference;

    public InferTypeHandler(ITypeInferenceService typeInference)
    {
        _typeInference = typeInference;
    }

    public Task<TypeInferenceResult> Handle(InferTypeQuery request, CancellationToken cancellationToken)
        => _typeInference.InferAsync(request.Text, cancellationToken);
}
