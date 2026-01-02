using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Collections.Queries;

public sealed record GetCollectionByIdQuery(string Id) : IRequest<CollectionDto?>;
