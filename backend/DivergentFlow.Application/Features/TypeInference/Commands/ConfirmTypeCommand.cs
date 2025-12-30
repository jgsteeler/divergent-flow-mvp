using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.TypeInference.Commands;

public sealed record ConfirmTypeCommand(TypeConfirmationRequest Request) : IRequest<Unit>;
