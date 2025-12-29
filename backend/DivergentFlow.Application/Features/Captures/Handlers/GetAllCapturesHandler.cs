using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Captures.Queries;
using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Captures.Handlers;

public sealed class GetAllCapturesHandler : IRequestHandler<GetAllCapturesQuery, IReadOnlyList<CaptureDto>>
{
    private readonly ICaptureRepository _repository;
    private readonly IMapper _mapper;

    public GetAllCapturesHandler(ICaptureRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CaptureDto>> Handle(GetAllCapturesQuery request, CancellationToken cancellationToken)
    {
        var captures = await _repository.GetAllAsync(cancellationToken);
        return captures.Select(c => _mapper.Map<CaptureDto>(c)).ToList();
    }
}
