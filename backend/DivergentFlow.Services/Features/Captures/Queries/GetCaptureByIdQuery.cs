using DivergentFlow.Services.Models;
using MediatR;

namespace DivergentFlow.Services.Features.Captures.Queries;

public sealed record GetCaptureByIdQuery(string Id) : IRequest<CaptureDto?>;
