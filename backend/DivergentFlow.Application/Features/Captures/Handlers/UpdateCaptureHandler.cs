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
        var updated = new Capture
        {
            Id = request.Id,
            Text = request.Text,
            CreatedAt = 0,
            InferredType = request.InferredType,
            TypeConfidence = request.TypeConfidence
        };

        var saved = await _repository.UpdateAsync(request.Id, updated, cancellationToken);
        return saved is null ? null : _mapper.Map<CaptureDto>(saved);
    }
}
