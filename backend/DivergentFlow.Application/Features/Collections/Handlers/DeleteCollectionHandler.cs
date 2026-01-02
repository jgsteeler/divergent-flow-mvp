using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Collections.Commands;
using MediatR;

namespace DivergentFlow.Application.Features.Collections.Handlers;

public sealed class DeleteCollectionHandler : IRequestHandler<DeleteCollectionCommand, bool>
{
    private readonly ICollectionRepository _repository;

    public DeleteCollectionHandler(ICollectionRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(DeleteCollectionCommand request, CancellationToken cancellationToken)
        => _repository.DeleteAsync(request.Id, cancellationToken);
}
