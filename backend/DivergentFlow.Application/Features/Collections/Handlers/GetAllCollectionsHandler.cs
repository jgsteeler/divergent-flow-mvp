using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Collections.Queries;
using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Collections.Handlers;

public sealed class GetAllCollectionsHandler : IRequestHandler<GetAllCollectionsQuery, IReadOnlyList<CollectionDto>>
{
    private readonly ICollectionRepository _repository;
    private readonly IMapper _mapper;

    public GetAllCollectionsHandler(ICollectionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CollectionDto>> Handle(GetAllCollectionsQuery request, CancellationToken cancellationToken)
    {
        var collections = await _repository.GetAllAsync(cancellationToken);
        return collections.Select(c => _mapper.Map<CollectionDto>(c)).ToList();
    }
}
