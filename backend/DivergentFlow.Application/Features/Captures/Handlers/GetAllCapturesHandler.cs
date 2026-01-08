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
    private readonly IUserContext _userContext;

    public GetAllCapturesHandler(ICaptureRepository repository, IMapper mapper, IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<CaptureDto>> Handle(GetAllCapturesQuery request, CancellationToken cancellationToken)
    {
        var captures = await _repository.GetAllAsync(_userContext.UserId, cancellationToken);
        return captures.Select(c => _mapper.Map<CaptureDto>(c)).ToList();
    }
}
