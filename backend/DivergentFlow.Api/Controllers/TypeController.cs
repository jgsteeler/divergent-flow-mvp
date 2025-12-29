using Microsoft.AspNetCore.Mvc;
using DivergentFlow.Application.Features.TypeInference.Commands;
using DivergentFlow.Application.Features.TypeInference.Queries;
using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Api.Controllers;

/// <summary>
/// Controller for type inference operations
/// Provides endpoints for inferring types and confirming user selections
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TypeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TypeController> _logger;

    public TypeController(
        IMediator mediator,
        ILogger<TypeController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Infer the type of captured text
    /// </summary>
    /// <param name="request">The text to analyze</param>
    /// <returns>The inferred type and confidence level</returns>
    [HttpPost("infer")]
    [ProducesResponseType(typeof(TypeInferenceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TypeInferenceResult>> Infer([FromBody] TypeInferenceRequest request)
    {
        _logger.LogInformation("Inferring type for capture text");

        var result = await _mediator.Send(new InferTypeQuery(request.Text ?? string.Empty));
        return Ok(result);
    }

    /// <summary>
    /// Confirm the type of a capture for learning purposes
    /// </summary>
    /// <param name="request">The confirmation details</param>
    /// <returns>No content on success</returns>
    [HttpPost("confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Confirm([FromBody] TypeConfirmationRequest request)
    {
        _logger.LogInformation("Confirming type for capture");

        await _mediator.Send(new ConfirmTypeCommand(request));
        return NoContent();
    }
}
