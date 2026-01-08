using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Collections.Commands;
using DivergentFlow.Application.Models;
using DivergentFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DivergentFlow.Application.Features.Collections.Handlers;

public sealed class CreateCollectionHandler : IRequestHandler<CreateCollectionCommand, CollectionDto>
{
    private readonly ICollectionRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCollectionHandler> _logger;
    private readonly IUserContext _userContext;

    public CreateCollectionHandler(
        ICollectionRepository repository,
        IMapper mapper,
        ILogger<CreateCollectionHandler> logger,
        IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<CollectionDto> Handle(CreateCollectionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling CreateCollectionCommand with name={Name}", request.Name);

        var collection = new Collection
        {
            UserId = _userContext.UserId,
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        var created = await _repository.CreateAsync(_userContext.UserId, collection, cancellationToken);
        return _mapper.Map<CollectionDto>(created);
    }
}
