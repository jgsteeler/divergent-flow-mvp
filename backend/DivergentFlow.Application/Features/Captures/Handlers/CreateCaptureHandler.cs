using AutoMapper;
using DivergentFlow.Application.Abstractions;
using DivergentFlow.Application.Features.Captures.Commands;
using DivergentFlow.Application.Models;
using DivergentFlow.Domain.Entities;
using MediatR;

namespace DivergentFlow.Application.Features.Captures.Handlers;

/// <summary>
/// Handles <see cref="CreateCaptureCommand"/> requests by creating a new <see cref="Capture"/>
/// entity and returning its <see cref="CaptureDto"/> representation.
/// </summary>
public sealed class CreateCaptureHandler : IRequestHandler<CreateCaptureCommand, CaptureDto>
{
    private readonly ICaptureRepository _repository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCaptureHandler"/> class.
    /// </summary>
    /// <param name="repository">The capture repository used to persist new captures.</param>
    /// <param name="mapper">The mapper used to convert entities to data transfer objects.</param>
    public CreateCaptureHandler(ICaptureRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the <see cref="CreateCaptureCommand"/> by creating a new capture and
    /// returning the created capture as a <see cref="CaptureDto"/>.
    /// </summary>
    /// <param name="request">The command containing the data for the capture to create.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// created capture as a <see cref="CaptureDto"/>.
    /// </returns>
    public async Task<CaptureDto> Handle(CreateCaptureCommand request, CancellationToken cancellationToken)
    {
        var capture = new Capture
        {
            Id = Guid.NewGuid().ToString(),
            Text = request.Text,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            InferredType = request.InferredType,
            TypeConfidence = request.TypeConfidence
        };

        var created = await _repository.CreateAsync(capture, cancellationToken);
        return _mapper.Map<CaptureDto>(created);
    }
}
