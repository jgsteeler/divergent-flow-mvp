using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Collections.Commands;
using DivergentFlow.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DivergentFlow.Application.Features.Collections.Handlers;

public sealed class UpdateCollectionHandler : IRequestHandler<UpdateCollectionCommand, CollectionDto?>
{
    private readonly ICollectionRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCollectionHandler> _logger;

    public UpdateCollectionHandler(
        ICollectionRepository repository,
        IMapper mapper,
        ILogger<UpdateCollectionHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CollectionDto?> Handle(UpdateCollectionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling UpdateCollectionCommand for collection {CollectionId}", request.Id);

        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing == null)
        {
            _logger.LogWarning("Collection {CollectionId} not found", request.Id);
            return null;
        }

        existing.Name = request.Name;

        var updated = await _repository.UpdateAsync(request.Id, existing, cancellationToken);
        return updated == null ? null : _mapper.Map<CollectionDto>(updated);
    }
}
