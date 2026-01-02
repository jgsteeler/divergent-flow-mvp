using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Items.Commands;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Handlers;

public sealed class DeleteItemHandler : IRequestHandler<DeleteItemCommand, bool>
{
    private readonly IItemRepository _repository;

    public DeleteItemHandler(IItemRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        => _repository.DeleteAsync(request.Id, cancellationToken);
}
