using AutoMapper;
using DivergentFlow.Application.Models;
using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Application.Features.Items.Mapping;

public sealed class ItemMappingProfile : Profile
{
    public ItemMappingProfile()
    {
        CreateMap<Item, ItemDto>().ReverseMap();
    }
}
