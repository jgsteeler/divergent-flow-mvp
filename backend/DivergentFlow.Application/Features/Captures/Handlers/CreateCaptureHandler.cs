using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Captures.Commands;
using DivergentFlow.Application.Models;
using DivergentFlow.Domain.Entities;
using MediatR;

namespace DivergentFlow.Application.Features.Captures.Handlers;





public sealed class CreateCaptureHandler : IRequestHandler<CreateCaptureCommand, CaptureDto>
{
    private readonly ICaptureRepository _repository;
    private readonly IMapper _mapper;

    
    
    
    
    
    public CreateCaptureHandler(ICaptureRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    
    
    
    
    
    
    
    
    
    
    public async Task<CaptureDto> Handle(CreateCaptureCommand request, CancellationToken cancellationToken)
    {
        var capture = new Capture
        {
            Id = Guid.NewGuid().ToString(),
            Text = request.Text,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            InferredType = request.InferredType,
            TypeConfidence = request.TypeConfidence
        };

        var created = await _repository.CreateAsync(capture, cancellationToken);
        return _mapper.Map<CaptureDto>(created);
    }
}
