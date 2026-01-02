using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Collections.Commands;

public sealed record CreateCollectionCommand(string Name) : IRequest<CollectionDto>;
