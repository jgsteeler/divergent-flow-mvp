using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Collections.Commands;

public sealed record UpdateCollectionCommand(string Id, string Name) : IRequest<CollectionDto?>;
