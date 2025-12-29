using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.TypeInference.Commands;
using MediatR;

namespace DivergentFlow.Application.Features.TypeInference.Handlers;

public sealed class ConfirmTypeHandler : IRequestHandler<ConfirmTypeCommand, Unit>
{
    private readonly ITypeInferenceService _typeInference;

    public ConfirmTypeHandler(ITypeInferenceService typeInference)
    {
        _typeInference = typeInference;
    }

    public async Task<Unit> Handle(ConfirmTypeCommand request, CancellationToken cancellationToken)
    {
        await _typeInference.ConfirmAsync(request.Request, cancellationToken);
        return Unit.Value;
    }
}
