using Microsoft.AspNetCore.Mvc;
using DivergentFlow.Application.Features.Collections.Commands;
using DivergentFlow.Application.Features.Collections.Queries;
using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Api.Controllers;

/// <summary>
/// Controller for managing collections
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CollectionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CollectionsController> _logger;

    public CollectionsController(
        IMediator mediator,
        ILogger<CollectionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all collections
    /// </summary>
    /// <returns>List of all collections</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CollectionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CollectionDto>>> GetAll()
    {
        _logger.LogInformation("Getting all collections");
        var collections = await _mediator.Send(new GetAllCollectionsQuery());
        return Ok(collections);
    }

    /// <summary>
    /// Get a specific collection by ID
    /// </summary>
    /// <param name="id">The collection ID</param>
    /// <returns>The requested collection</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CollectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CollectionDto>> GetById(string id)
    {
        _logger.LogInformation("Getting collection with ID: {Id}", id);
        var collection = await _mediator.Send(new GetCollectionByIdQuery(id));

        if (collection == null)
        {
            _logger.LogWarning("Collection with ID {Id} not found", id);
            return NotFound();
        }

        return Ok(collection);
    }

    /// <summary>
    /// Create a new collection
    /// </summary>
    /// <param name="request">The collection data</param>
    /// <returns>The created collection</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CollectionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CollectionDto>> Create([FromBody] CreateCollectionRequest request)
    {
        _logger.LogInformation("Creating new collection");

        var collection = await _mediator.Send(new CreateCollectionCommand(request.Name));
        return CreatedAtAction(nameof(GetById), new { id = collection.Id }, collection);
    }

    /// <summary>
    /// Update an existing collection
    /// </summary>
    /// <param name="id">The collection ID</param>
    /// <param name="request">The updated collection data</param>
    /// <returns>The updated collection</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CollectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CollectionDto>> Update(string id, [FromBody] UpdateCollectionRequest request)
    {
        _logger.LogInformation("Updating collection with ID: {Id}", id);

        var collection = await _mediator.Send(new UpdateCollectionCommand(id, request.Name));

        if (collection == null)
        {
            _logger.LogWarning("Collection with ID {Id} not found", id);
            return NotFound();
        }

        return Ok(collection);
    }

    /// <summary>
    /// Delete a collection
    /// </summary>
    /// <param name="id">The collection ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        _logger.LogInformation("Deleting collection with ID: {Id}", id);

        var deleted = await _mediator.Send(new DeleteCollectionCommand(id));

        if (!deleted)
        {
            _logger.LogWarning("Collection with ID {Id} not found", id);
            return NotFound();
        }

        return NoContent();
    }
}
