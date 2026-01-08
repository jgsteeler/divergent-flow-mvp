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
    private readonly ITypeInferenceWorkflowTrigger _workflowTrigger;
    private readonly IUserContext _userContext;

    public UpdateItemHandler(
        IItemRepository repository,
        IMapper mapper,
        ILogger<UpdateItemHandler> logger,
        ITypeInferenceWorkflowTrigger workflowTrigger,
        IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _workflowTrigger = workflowTrigger;
        _userContext = userContext;
    }

    public async Task<ItemDto?> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling UpdateItemCommand for item {ItemId}", request.Id);

        var existing = await _repository.GetByIdAsync(_userContext.UserId, request.Id, cancellationToken);
        if (existing == null)
        {
            _logger.LogWarning("Item {ItemId} not found", request.Id);
            return null;
        }

        // Track if text changed to trigger re-inference
        var textChanged = !string.Equals(existing.Text, request.Text, StringComparison.Ordinal);

        existing.Text = request.Text;
        existing.InferredType = request.InferredType;
        existing.TypeConfidence = request.TypeConfidence;
        existing.CollectionId = request.CollectionId;

        var updated = await _repository.UpdateAsync(_userContext.UserId, request.Id, existing, cancellationToken);
        
        // Trigger re-inference if text changed (non-blocking)
        if (updated is not null && textChanged)
        {
            _logger.LogDebug("Text changed for item {ItemId}, triggering re-inference", request.Id);
            _workflowTrigger.TriggerInferenceWorkflow(_userContext.UserId, updated.Id);
        }
        
        return updated == null ? null : _mapper.Map<ItemDto>(updated);
    }
}
