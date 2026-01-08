using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Captures.Queries;
using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Captures.Handlers;

public sealed class GetCaptureByIdHandler : IRequestHandler<GetCaptureByIdQuery, CaptureDto?>
{
    private readonly ICaptureRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetCaptureByIdHandler(ICaptureRepository repository, IMapper mapper, IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<CaptureDto?> Handle(GetCaptureByIdQuery request, CancellationToken cancellationToken)
    {
        var capture = await _repository.GetByIdAsync(_userContext.UserId, request.Id, cancellationToken);
        return capture is null ? null : _mapper.Map<CaptureDto>(capture);
    }
}
