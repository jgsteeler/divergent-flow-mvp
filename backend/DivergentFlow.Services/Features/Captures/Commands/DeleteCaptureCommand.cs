using MediatR;

namespace DivergentFlow.Services.Features.Captures.Commands;

public sealed record DeleteCaptureCommand(string Id) : IRequest<bool>;
