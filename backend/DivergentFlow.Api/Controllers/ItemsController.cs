using Microsoft.AspNetCore.Mvc;
using DivergentFlow.Application.Features.Items.Commands;
using DivergentFlow.Application.Features.Items.Queries;
using DivergentFlow.Application.Models;
using MediatR;

namespace DivergentFlow.Api.Controllers;

/// <summary>
/// Controller for managing items
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ItemsController> _logger;

    public ItemsController(
        IMediator mediator,
        ILogger<ItemsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all items
    /// </summary>
    /// <returns>List of all items</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAll()
    {
        _logger.LogInformation("Getting all items");
        var items = await _mediator.Send(new GetAllItemsQuery());
        return Ok(items);
    }

    /// <summary>
    /// Get a specific item by ID
    /// </summary>
    /// <param name="id">The item ID</param>
    /// <returns>The requested item</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemDto>> GetById(string id)
    {
        _logger.LogInformation("Getting item with ID: {Id}", id);
        var item = await _mediator.Send(new GetItemByIdQuery(id));

        if (item == null)
        {
            _logger.LogWarning("Item with ID {Id} not found", id);
            return NotFound();
        }

        return Ok(item);
    }

    /// <summary>
    /// Create a new item
    /// </summary>
    /// <param name="request">The item data</param>
    /// <returns>The created item</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ItemDto>> Create([FromBody] CreateItemRequest request)
    {
        _logger.LogInformation("Creating new item");

        var item = await _mediator.Send(new CreateItemCommand(
            Text: request.Text,
            InferredType: request.InferredType,
            TypeConfidence: request.TypeConfidence,
            CollectionId: request.CollectionId
        ));
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    /// <summary>
    /// Update an existing item
    /// </summary>
    /// <param name="id">The item ID</param>
    /// <param name="request">The updated item data</param>
    /// <returns>The updated item</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemDto>> Update(string id, [FromBody] UpdateItemRequest request)
    {
        _logger.LogInformation("Updating item with ID: {Id}", id);

        var item = await _mediator.Send(new UpdateItemCommand(
            Id: id,
            Text: request.Text,
            InferredType: request.InferredType,
            TypeConfidence: request.TypeConfidence,
            CollectionId: request.CollectionId
        ));

        if (item == null)
        {
            _logger.LogWarning("Item with ID {Id} not found", id);
            return NotFound();
        }

        return Ok(item);
    }

    /// <summary>
    /// Delete an item
    /// </summary>
    /// <param name="id">The item ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        _logger.LogInformation("Deleting item with ID: {Id}", id);

        var deleted = await _mediator.Send(new DeleteItemCommand(id));

        if (!deleted)
        {
            _logger.LogWarning("Item with ID {Id} not found", id);
            return NotFound();
        }

        return NoContent();
    }
}
