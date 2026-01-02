using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Commands;

public sealed record UpdateItemCommand(
    string Id,
    string Text,
    string? InferredType,
    double? TypeConfidence,
    string? CollectionId
) : IRequest<ItemDto?>;
