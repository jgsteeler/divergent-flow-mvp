using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Collections.Commands;
using MediatR;

namespace DivergentFlow.Application.Features.Collections.Handlers;

public sealed class DeleteCollectionHandler : IRequestHandler<DeleteCollectionCommand, bool>
{
    private readonly ICollectionRepository _repository;
    private readonly IUserContext _userContext;

    public DeleteCollectionHandler(ICollectionRepository repository, IUserContext userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }

    public Task<bool> Handle(DeleteCollectionCommand request, CancellationToken cancellationToken)
        => _repository.DeleteAsync(_userContext.UserId, request.Id, cancellationToken);
}
