using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Commands;

public sealed record CreateItemCommand(
    string Text,
    string? InferredType,
    double? TypeConfidence,
    string? CollectionId
) : IRequest<ItemDto>;
