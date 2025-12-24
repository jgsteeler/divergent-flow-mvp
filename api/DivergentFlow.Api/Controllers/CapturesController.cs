using Microsoft.AspNetCore.Mvc;
using DivergentFlow.Api.Models;
using DivergentFlow.Api.Services;

namespace DivergentFlow.Api.Controllers;

/// <summary>
/// Controller for managing capture items
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CapturesController : ControllerBase
{
    private readonly ICaptureService _captureService;
    private readonly ILogger<CapturesController> _logger;

    public CapturesController(
        ICaptureService captureService,
        ILogger<CapturesController> logger)
    {
        _captureService = captureService;
        _logger = logger;
    }

    /// <summary>
    /// Get all captures
    /// </summary>
    /// <returns>List of all captures</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CaptureDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CaptureDto>>> GetAll()
    {
        _logger.LogInformation("Getting all captures");
        var captures = await _captureService.GetAllAsync();
        return Ok(captures);
    }

    /// <summary>
    /// Get a specific capture by ID
    /// </summary>
    /// <param name="id">The capture ID</param>
    /// <returns>The requested capture</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CaptureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CaptureDto>> GetById(string id)
    {
        _logger.LogInformation("Getting capture with ID: {Id}", id);
        var capture = await _captureService.GetByIdAsync(id);

        if (capture == null)
        {
            _logger.LogWarning("Capture with ID {Id} not found", id);
            return NotFound();
        }

        return Ok(capture);
    }

    /// <summary>
    /// Create a new capture
    /// </summary>
    /// <param name="request">The capture data</param>
    /// <returns>The created capture</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CaptureDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CaptureDto>> Create([FromBody] CreateCaptureRequest request)
    {
        _logger.LogInformation("Creating new capture");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var capture = await _captureService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = capture.Id }, capture);
    }

    /// <summary>
    /// Update an existing capture
    /// </summary>
    /// <param name="id">The capture ID</param>
    /// <param name="request">The updated capture data</param>
    /// <returns>The updated capture</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CaptureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CaptureDto>> Update(string id, [FromBody] UpdateCaptureRequest request)
    {
        _logger.LogInformation("Updating capture with ID: {Id}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var capture = await _captureService.UpdateAsync(id, request);

        if (capture == null)
        {
            _logger.LogWarning("Capture with ID {Id} not found", id);
            return NotFound();
        }

        return Ok(capture);
    }

    /// <summary>
    /// Delete a capture
    /// </summary>
    /// <param name="id">The capture ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        _logger.LogInformation("Deleting capture with ID: {Id}", id);

        var deleted = await _captureService.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("Capture with ID {Id} not found", id);
            return NotFound();
        }

        return NoContent();
    }
}
