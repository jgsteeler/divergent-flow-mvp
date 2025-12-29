using AutoMapper;
using DivergentFlow.Services.Domain;
using DivergentFlow.Services.Features.Captures.Commands;
using DivergentFlow.Services.Models;
using DivergentFlow.Services.Repositories;
using MediatR;

namespace DivergentFlow.Services.Features.Captures.Handlers;

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
