using DivergentFlow.Services.Models;
using MediatR;

namespace DivergentFlow.Services.Features.Captures.Commands;

public sealed record CreateCaptureCommand(
    string Text,
    string? InferredType,
    double? TypeConfidence
) : IRequest<CaptureDto>;
