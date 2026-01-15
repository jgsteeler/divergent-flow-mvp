using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Items.Queries;
using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Handlers;

/// <summary>
/// Handler for getting items that need review
/// </summary>
public sealed class GetReviewQueueHandler : IRequestHandler<GetReviewQueueQuery, IReadOnlyList<ItemDto>>
{
    private readonly IItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetReviewQueueHandler(IItemRepository repository, IMapper mapper, IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<ItemDto>> Handle(GetReviewQueueQuery request, CancellationToken cancellationToken)
    {
        // Get all items for the user
        var allItems = await _repository.GetAllAsync(_userContext.UserId, cancellationToken);
        
        // Filter and prioritize items for review
        var reviewQueue = allItems
            // Priority 1: Never reviewed
            .OrderBy(item => item.LastReviewedAt.HasValue ? 1 : 0)
            // Priority 2: Low confidence items (if specified)
            .ThenBy(item =>
            {
                if (!request.MaxConfidence.HasValue)
                    return 1;
                if (!item.TypeConfidence.HasValue)
                    return 0; // Null confidence = highest priority
                return item.TypeConfidence.Value > request.MaxConfidence.Value ? 1 : 0;
            })
            // Priority 3: Oldest first
            .ThenBy(item => item.CreatedAt)
            .Where(item =>
            {
                // Include items that have never been reviewed
                if (!item.LastReviewedAt.HasValue)
                    return true;
                
                // Include items with low confidence (if threshold specified)
                if (request.MaxConfidence.HasValue &&
                    item.TypeConfidence.HasValue &&
                    item.TypeConfidence.Value <= request.MaxConfidence.Value)
                    return true;
                
                // Include items with null confidence
                if (!item.TypeConfidence.HasValue)
                    return true;
                
                return false;
            })
            .Take(request.Limit)
            .ToList();
        
        return reviewQueue.Select(item => _mapper.Map<ItemDto>(item)).ToList();
    }
}
