using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Captures.Queries;

public sealed record GetAllCapturesQuery() : IRequest<IReadOnlyList<CaptureDto>>;
