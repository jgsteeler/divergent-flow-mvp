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
    private readonly IUserContext _userContext;

    public UpdateCaptureHandler(ICaptureRepository repository, IMapper mapper, IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<CaptureDto?> Handle(UpdateCaptureCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(_userContext.UserId, request.Id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Text = request.Text;
        existing.InferredType = request.InferredType;
        existing.TypeConfidence = request.TypeConfidence;
        existing.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var saved = await _repository.UpdateAsync(_userContext.UserId, request.Id, existing, cancellationToken);
        return saved is null ? null : _mapper.Map<CaptureDto>(saved);
    }
}
