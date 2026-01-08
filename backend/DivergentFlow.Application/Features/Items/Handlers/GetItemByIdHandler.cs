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
    private readonly IUserContext _userContext;

    public GetItemByIdHandler(IItemRepository repository, IMapper mapper, IUserContext userContext)
    {
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<ItemDto?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(_userContext.UserId, request.Id, cancellationToken);
        return item == null ? null : _mapper.Map<ItemDto>(item);
    }
}
