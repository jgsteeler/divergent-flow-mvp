using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Items.Queries;
using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Handlers;

public sealed class GetItemByIdHandler : IRequestHandler<GetItemByIdQuery, ItemDto?>
{
    private readonly IItemRepository _repository;
    private readonly IMapper _mapper;

    public GetItemByIdHandler(IItemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ItemDto?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return item == null ? null : _mapper.Map<ItemDto>(item);
    }
}
