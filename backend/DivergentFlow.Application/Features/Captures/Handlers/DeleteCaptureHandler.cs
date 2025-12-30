using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Captures.Commands;
using MediatR;

namespace DivergentFlow.Application.Features.Captures.Handlers;

public sealed class DeleteCaptureHandler : IRequestHandler<DeleteCaptureCommand, bool>
{
    private readonly ICaptureRepository _repository;

    public DeleteCaptureHandler(ICaptureRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(DeleteCaptureCommand request, CancellationToken cancellationToken)
        => _repository.DeleteAsync(request.Id, cancellationToken);
}
