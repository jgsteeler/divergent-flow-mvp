using MediatR;

namespace DivergentFlow.Application.Features.Captures.Commands;

public sealed record DeleteCaptureCommand(string Id) : IRequest<bool>;
