using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Collections.Queries;

public sealed record GetAllCollectionsQuery : IRequest<IReadOnlyList<CollectionDto>>;
