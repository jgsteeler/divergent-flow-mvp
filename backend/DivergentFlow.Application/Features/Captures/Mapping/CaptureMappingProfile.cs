using AutoMapper;
using DivergentFlow.Application.Models;
using DivergentFlow.Domain.Entities;

namespace DivergentFlow.Application.Features.Captures.Mapping;

public sealed class CaptureMappingProfile : Profile
{
    public CaptureMappingProfile()
    {
        CreateMap<Capture, CaptureDto>();
    }
}
