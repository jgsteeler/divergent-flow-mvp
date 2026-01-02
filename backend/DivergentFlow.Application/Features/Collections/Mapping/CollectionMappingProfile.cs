using AutoMapper;
using DivergentFlow.Application.Models;
using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Application.Features.Collections.Mapping;

public sealed class CollectionMappingProfile : Profile
{
    public CollectionMappingProfile()
    {
        CreateMap<Collection, CollectionDto>().ReverseMap();
    }
}
