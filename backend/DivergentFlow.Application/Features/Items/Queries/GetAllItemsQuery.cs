using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Queries;

public sealed record GetAllItemsQuery : IRequest<IReadOnlyList<ItemDto>>;
