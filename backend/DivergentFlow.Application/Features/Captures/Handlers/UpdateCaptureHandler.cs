using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Captures.Commands;
using DivergentFlow.Application.Models;
using DivergentFlow.Domain.Entities;
using MediatR;

namespace DivergentFlow.Application.Features.Captures.Handlers;

public sealed class UpdateCaptureHandler : IRequestHandler<UpdateCaptureCommand, CaptureDto?>
{
    private readonly ICaptureRepository _repository;
    private readonly IMapper _mapper;

    public UpdateCaptureHandler(ICaptureRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CaptureDto?> Handle(UpdateCaptureCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Text = request.Text;
        existing.InferredType = request.InferredType;
        existing.TypeConfidence = request.TypeConfidence;

        var saved = await _repository.UpdateAsync(request.Id, existing, cancellationToken);
        return saved is null ? null : _mapper.Map<CaptureDto>(saved);
    }
}
