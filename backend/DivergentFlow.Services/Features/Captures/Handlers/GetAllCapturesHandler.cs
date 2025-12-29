using AutoMapper;
using DivergentFlow.Services.Features.Captures.Queries;
using DivergentFlow.Services.Models;
using DivergentFlow.Services.Repositories;
using MediatR;

namespace DivergentFlow.Services.Features.Captures.Handlers;

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
