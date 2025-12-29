using DivergentFlow.Services.Models;
using MediatR;

namespace DivergentFlow.Services.Features.Captures.Queries;

public sealed record GetAllCapturesQuery() : IRequest<IReadOnlyList<CaptureDto>>;
