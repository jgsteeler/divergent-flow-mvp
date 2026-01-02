using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Collections.Queries;
using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Collections.Handlers;

public sealed class GetCollectionByIdHandler : IRequestHandler<GetCollectionByIdQuery, CollectionDto?>
{
    private readonly ICollectionRepository _repository;
    private readonly IMapper _mapper;

    public GetCollectionByIdHandler(ICollectionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CollectionDto?> Handle(GetCollectionByIdQuery request, CancellationToken cancellationToken)
    {
        var collection = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return collection == null ? null : _mapper.Map<CollectionDto>(collection);
    }
}
