using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Queries;

public sealed record GetItemByIdQuery(string Id) : IRequest<ItemDto?>;
