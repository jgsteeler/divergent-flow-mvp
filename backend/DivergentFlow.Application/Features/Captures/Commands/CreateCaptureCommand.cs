using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Captures.Commands;

public sealed record CreateCaptureCommand(
    string Text,
    string? InferredType,
    double? TypeConfidence
) : IRequest<CaptureDto>;
