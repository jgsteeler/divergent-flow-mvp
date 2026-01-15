using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Commands;

/// <summary>
/// Command to mark an item as reviewed
/// </summary>
public sealed record MarkItemReviewedCommand(
    string Id,
    string? ConfirmedType = null,
    double? ConfirmedConfidence = null
) : IRequest<ItemDto?>;
