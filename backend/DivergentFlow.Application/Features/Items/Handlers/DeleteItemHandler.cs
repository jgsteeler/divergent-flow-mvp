using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Items.Commands;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Handlers;

public sealed class DeleteItemHandler : IRequestHandler<DeleteItemCommand, bool>
{
    private readonly IItemRepository _repository;
    private readonly IUserContext _userContext;

    public DeleteItemHandler(IItemRepository repository, IUserContext userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }

    public Task<bool> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        => _repository.DeleteAsync(_userContext.UserId, request.Id, cancellationToken);
}
