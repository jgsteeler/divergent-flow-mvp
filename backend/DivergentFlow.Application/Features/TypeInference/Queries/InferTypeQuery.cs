using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.TypeInference.Queries;

public sealed record InferTypeQuery(string Text) : IRequest<TypeInferenceResult>;
