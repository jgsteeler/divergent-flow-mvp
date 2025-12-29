using Microsoft.AspNetCore.Mvc;
using DivergentFlow.Services.Models;
using DivergentFlow.Services.Services;

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
    private readonly ITypeService _typeService;
    private readonly ILogger<TypeController> _logger;

    public TypeController(
        ITypeService typeService,
        ILogger<TypeController> logger)
    {
        _typeService = typeService;
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Inferring type for capture text");

        var result = await _typeService.InferAsync(request.Text!);
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Confirming type for capture");

        await _typeService.ConfirmAsync(request);
        return NoContent();
    }
}
