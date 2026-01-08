using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Captures.Commands;
using MediatR;

namespace DivergentFlow.Application.Features.Captures.Handlers;

public sealed class DeleteCaptureHandler : IRequestHandler<DeleteCaptureCommand, bool>
{
    private readonly ICaptureRepository _repository;
    private readonly IUserContext _userContext;

    public DeleteCaptureHandler(ICaptureRepository repository, IUserContext userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }

    public Task<bool> Handle(DeleteCaptureCommand request, CancellationToken cancellationToken)
        => _repository.DeleteAsync(_userContext.UserId, request.Id, cancellationToken);
}
