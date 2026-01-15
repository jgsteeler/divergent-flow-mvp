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
    /// Get items that need review
    /// </summary>
    /// <param name="limit">Maximum number of items to return (default: 3)</param>
    /// <param name="maxConfidence">Maximum confidence threshold for including items (default: 0.75)</param>
    /// <returns>List of items needing review</returns>
    [HttpGet("review-queue")]
    [ProducesResponseType(typeof(IEnumerable<ItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetReviewQueue(
        [FromQuery] int limit = 3,
        [FromQuery] double? maxConfidence = 0.75)
    {
        _logger.LogInformation("Getting review queue (limit: {Limit}, maxConfidence: {MaxConfidence})", limit, maxConfidence);
        var items = await _mediator.Send(new GetReviewQueueQuery(limit, maxConfidence));
        return Ok(items);
    }

    /// <summary>
    /// Get dashboard data including metrics and task lists
    /// </summary>
    /// <returns>Dashboard data</returns>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardDataDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardDataDto>> GetDashboard()
    {
        _logger.LogInformation("Getting dashboard data");
        var dashboardData = await _mediator.Send(new GetDashboardDataQuery());
        return Ok(dashboardData);
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
    /// Mark an item as reviewed
    /// </summary>
    /// <param name="id">The item ID</param>
    /// <param name="request">The review data (optional confirmed type/confidence)</param>
    /// <returns>The updated item</returns>
    [HttpPut("{id}/review")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemDto>> MarkReviewed(string id, [FromBody] MarkItemReviewedRequest? request = null)
    {
        _logger.LogInformation("Marking item {Id} as reviewed", id);

        var item = await _mediator.Send(new MarkItemReviewedCommand(
            Id: id,
            ConfirmedType: request?.ConfirmedType,
            ConfirmedConfidence: request?.ConfirmedConfidence
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
