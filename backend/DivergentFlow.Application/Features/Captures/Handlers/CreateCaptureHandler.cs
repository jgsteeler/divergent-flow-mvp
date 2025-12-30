using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Captures.Commands;
using DivergentFlow.Application.Models;
using DivergentFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DivergentFlow.Application.Features.Captures.Handlers;





public sealed class CreateCaptureHandler : IRequestHandler<CreateCaptureCommand, CaptureDto>
{
    private readonly ICaptureRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCaptureHandler> _logger;
    private readonly ITypeInferenceWorkflowTrigger _workflowTrigger;

    
    
    
    
    
    public CreateCaptureHandler(
        ICaptureRepository repository,
        IMapper mapper,
        ILogger<CreateCaptureHandler> logger,
        ITypeInferenceWorkflowTrigger workflowTrigger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _workflowTrigger = workflowTrigger;
    }

    
    
    
    
    
    
    
    
    
    
    public async Task<CaptureDto> Handle(CreateCaptureCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug(
            "Handling CreateCaptureCommand via {RepositoryType}. textLength={TextLength} inferredType={InferredType} typeConfidence={TypeConfidence}",
            _repository.GetType().FullName,
            request.Text?.Length ?? 0,
            request.InferredType,
            request.TypeConfidence);

        var capture = new Capture
        {
            Id = Guid.NewGuid().ToString(),
            Text = request.Text,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            InferredType = request.InferredType,
            TypeConfidence = request.TypeConfidence,
            IsMigrated = false
        };

        _logger.LogDebug(
            "Creating capture {CaptureId} at {CreatedAtUnixMs}",
            capture.Id,
            capture.CreatedAt);

        var created = await _repository.CreateAsync(capture, cancellationToken);

        _logger.LogDebug(
            "CreateAsync returned capture {CaptureId} (sameId={SameId})",
            created.Id,
            string.Equals(created.Id, capture.Id, StringComparison.Ordinal));

        // Trigger background re-inference workflow (non-blocking)
        _workflowTrigger.TriggerInferenceWorkflow(created.Id);

        return _mapper.Map<CaptureDto>(created);
    }
}
