using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Captures.Commands;

public sealed record UpdateCaptureCommand(
    string Id,
    string Text,
    string? InferredType,
    double? TypeConfidence
) : IRequest<CaptureDto?>;
