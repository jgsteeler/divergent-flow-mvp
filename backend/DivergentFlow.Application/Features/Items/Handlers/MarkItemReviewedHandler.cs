using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Items.Commands;
using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Handlers;

/// <summary>
/// Handler for marking an item as reviewed
/// </summary>
public sealed class MarkItemReviewedHandler : IRequestHandler<MarkItemReviewedCommand, ItemDto?>
{
    private readonly IItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public MarkItemReviewedHandler(IItemRepository repository, IMapper mapper, IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<ItemDto?> Handle(MarkItemReviewedCommand request, CancellationToken cancellationToken)
    {
        // Get the existing item
        var item = await _repository.GetByIdAsync(_userContext.UserId, request.Id, cancellationToken);
        
        if (item == null)
            return null;
        
        // Update review timestamp
        item.LastReviewedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        
        // If user confirmed/corrected the type, update it
        if (request.ConfirmedType != null)
        {
            item.InferredType = request.ConfirmedType;
        }
        
        if (request.ConfirmedConfidence.HasValue)
        {
            item.TypeConfidence = request.ConfirmedConfidence.Value;
        }
        
        // Save the updated item
        var updatedItem = await _repository.UpdateAsync(_userContext.UserId, request.Id, item, cancellationToken);
        
        return updatedItem == null ? null : _mapper.Map<ItemDto>(updatedItem);
    }
}
