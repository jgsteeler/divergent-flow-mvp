using AutoMapper;
using DivergentFlow.Services.Features.Captures.Queries;
using DivergentFlow.Services.Models;
using DivergentFlow.Services.Repositories;
using MediatR;

namespace DivergentFlow.Services.Features.Captures.Handlers;

public sealed class GetCaptureByIdHandler : IRequestHandler<GetCaptureByIdQuery, CaptureDto?>
{
    private readonly ICaptureRepository _repository;
    private readonly IMapper _mapper;

    public GetCaptureByIdHandler(ICaptureRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CaptureDto?> Handle(GetCaptureByIdQuery request, CancellationToken cancellationToken)
    {
        var capture = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return capture is null ? null : _mapper.Map<CaptureDto>(capture);
    }
}
