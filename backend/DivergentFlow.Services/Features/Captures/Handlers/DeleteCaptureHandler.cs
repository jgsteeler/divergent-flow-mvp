using DivergentFlow.Services.Features.Captures.Commands;
using DivergentFlow.Services.Repositories;
using MediatR;

namespace DivergentFlow.Services.Features.Captures.Handlers;

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
