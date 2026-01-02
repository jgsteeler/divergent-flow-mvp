using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Items.Commands;
using DivergentFlow.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DivergentFlow.Application.Features.Items.Handlers;

public sealed class UpdateItemHandler : IRequestHandler<UpdateItemCommand, ItemDto?>
{
    private readonly IItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateItemHandler> _logger;

    public UpdateItemHandler(
        IItemRepository repository,
        IMapper mapper,
        ILogger<UpdateItemHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ItemDto?> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling UpdateItemCommand for item {ItemId}", request.Id);

        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing == null)
        {
            _logger.LogWarning("Item {ItemId} not found", request.Id);
            return null;
        }

        existing.Text = request.Text;
        existing.InferredType = request.InferredType;
        existing.TypeConfidence = request.TypeConfidence;
        existing.CollectionId = request.CollectionId;

        var updated = await _repository.UpdateAsync(request.Id, existing, cancellationToken);
        return updated == null ? null : _mapper.Map<ItemDto>(updated);
    }
}
