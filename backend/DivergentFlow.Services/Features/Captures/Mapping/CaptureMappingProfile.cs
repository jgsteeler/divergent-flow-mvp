using AutoMapper;
using DivergentFlow.Services.Domain;
using DivergentFlow.Services.Models;

namespace DivergentFlow.Services.Features.Captures.Mapping;

public sealed class CaptureMappingProfile : Profile
{
    public CaptureMappingProfile()
    {
        CreateMap<Capture, CaptureDto>();
    }
}
