using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Items.Commands;
using DivergentFlow.Application.Models;
using DivergentFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DivergentFlow.Application.Features.Items.Handlers;

public sealed class CreateItemHandler : IRequestHandler<CreateItemCommand, ItemDto>
{
    private readonly IItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateItemHandler> _logger;
    private readonly ITypeInferenceWorkflowTrigger _workflowTrigger;
    private readonly IUserContext _userContext;

    public CreateItemHandler(
        IItemRepository repository,
        IMapper mapper,
        ILogger<CreateItemHandler> logger,
        ITypeInferenceWorkflowTrigger workflowTrigger,
        IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _workflowTrigger = workflowTrigger;
        _userContext = userContext;
    }

    public async Task<ItemDto> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "Handling CreateItemCommand via {RepositoryType}. textLength={TextLength} inferredType={InferredType} typeConfidence={TypeConfidence}",
            _repository.GetType().FullName,
            request.Text?.Length ?? 0,
            request.InferredType,
            request.TypeConfidence);

        var item = new Item
        {
            UserId = _userContext.UserId,
            Id = Guid.NewGuid().ToString(),
            Type = "capture", // Default type for all new items
            Text = request.Text,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            InferredType = request.InferredType,
            TypeConfidence = request.TypeConfidence,
            CollectionId = request.CollectionId,
            LastReviewedAt = null
        };

        _logger.LogDebug(
            "Creating item {ItemId} at {CreatedAtUnixMs}",
            item.Id,
            item.CreatedAt);

        var created = await _repository.CreateAsync(_userContext.UserId, item, cancellationToken);

        _logger.LogDebug(
            "CreateAsync returned item {ItemId} (sameId={SameId})",
            created.Id,
            string.Equals(created.Id, item.Id, StringComparison.Ordinal));

        // Trigger background re-inference workflow (non-blocking)
        _workflowTrigger.TriggerInferenceWorkflow(_userContext.UserId, created.Id);

        return _mapper.Map<ItemDto>(created);
    }
}
