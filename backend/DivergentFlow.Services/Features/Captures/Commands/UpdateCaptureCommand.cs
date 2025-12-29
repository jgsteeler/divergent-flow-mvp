using DivergentFlow.Services.Models;
using MediatR;

namespace DivergentFlow.Services.Features.Captures.Commands;

public sealed record UpdateCaptureCommand(
    string Id,
    string Text,
    string? InferredType,
    double? TypeConfidence
) : IRequest<CaptureDto?>;
