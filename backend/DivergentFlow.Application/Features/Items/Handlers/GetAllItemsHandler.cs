using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Items.Queries;
using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Application.Features.Items.Handlers;

public sealed class GetAllItemsHandler : IRequestHandler<GetAllItemsQuery, IReadOnlyList<ItemDto>>
{
    private readonly IItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetAllItemsHandler(IItemRepository repository, IMapper mapper, IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<ItemDto>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(_userContext.UserId, cancellationToken);
        return items.Select(i => _mapper.Map<ItemDto>(i)).ToList();
    }
}
