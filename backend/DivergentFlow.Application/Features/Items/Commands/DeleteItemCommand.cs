using MediatR;

namespace DivergentFlow.Application.Features.Items.Commands;

public sealed record DeleteItemCommand(string Id) : IRequest<bool>;
