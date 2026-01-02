using MediatR;

namespace DivergentFlow.Application.Features.Collections.Commands;

public sealed record DeleteCollectionCommand(string Id) : IRequest<bool>;
