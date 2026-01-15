using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Queries;

/// <summary>
/// Query to get items that need review, prioritized by confidence and review status
/// </summary>
public sealed record GetReviewQueueQuery(
    int Limit = 3,
    double? MaxConfidence = 0.75
) : IRequest<IReadOnlyList<ItemDto>>;
