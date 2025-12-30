using DivergentFlow.Api.Controllers;
using DivergentFlow.Api.Tests.TestDoubles;
using DivergentFlow.Application.Features.Captures.Commands;
using DivergentFlow.Application.Features.Captures.Queries;
using DivergentFlow.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DivergentFlow.Api.Tests;

public sealed class CapturesControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOk_WithList()
    {
        var expected = new List<CaptureDto>();
        var mediator = new FakeMediator((request, _) =>
        {
            Assert.IsType<GetAllCapturesQuery>(request);
            return Task.FromResult<object?>(expected);
        });

        var controller = new CapturesController(mediator, NullLogger<CapturesController>.Instance);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenExists()
    {
        var expected = new CaptureDto { Id = "capture-1", Text = "Test capture", CreatedAt = 123 };
        var mediator = new FakeMediator((request, _) =>
        {
            var query = Assert.IsType<GetCaptureByIdQuery>(request);
            Assert.Equal("capture-1", query.Id);
            return Task.FromResult<object?>(expected);
        });

        var controller = new CapturesController(mediator, NullLogger<CapturesController>.Instance);

        var result = await controller.GetById("capture-1");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var mediator = new FakeMediator((request, _) =>
        {
            Assert.IsType<GetCaptureByIdQuery>(request);
            return Task.FromResult<object?>(null);
        });

        var controller = new CapturesController(mediator, NullLogger<CapturesController>.Instance);

        var result = await controller.GetById("missing");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithCapture()
    {
        var request = new CreateCaptureRequest
        {
            Text = "Test capture",
            InferredType = "note",
            TypeConfidence = 95.5
        };

        var created = new CaptureDto
        {
            Id = "capture-1",
            Text = "Test capture",
            InferredType = "note",
            TypeConfidence = 95.5,
            CreatedAt = 123
        };

        var mediator = new FakeMediator((message, _) =>
        {
            var command = Assert.IsType<CreateCaptureCommand>(message);
            Assert.Equal("Test capture", command.Text);
            Assert.Equal("note", command.InferredType);
            Assert.Equal(95.5, command.TypeConfidence);
            return Task.FromResult<object?>(created);
        });

        var controller = new CapturesController(mediator, NullLogger<CapturesController>.Instance);

        var result = await controller.Create(request);

        var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<CaptureDto>(createdAt.Value);
        Assert.Equal("capture-1", dto.Id);
        Assert.Equal("Test capture", dto.Text);
        Assert.Equal("note", dto.InferredType);
        Assert.Equal(95.5, dto.TypeConfidence);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenExists()
    {
        var updateRequest = new UpdateCaptureRequest
        {
            Text = "Updated text",
            InferredType = "action",
            TypeConfidence = 87.3
        };

        var updated = new CaptureDto
        {
            Id = "capture-1",
            Text = "Updated text",
            InferredType = "action",
            TypeConfidence = 87.3,
            CreatedAt = 123
        };

        var mediator = new FakeMediator((message, _) =>
        {
            var command = Assert.IsType<UpdateCaptureCommand>(message);
            Assert.Equal("capture-1", command.Id);
            Assert.Equal("Updated text", command.Text);
            Assert.Equal("action", command.InferredType);
            Assert.Equal(87.3, command.TypeConfidence);
            return Task.FromResult<object?>(updated);
        });

        var controller = new CapturesController(mediator, NullLogger<CapturesController>.Instance);

        var result = await controller.Update("capture-1", updateRequest);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(updated, ok.Value);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenMissing()
    {
        var mediator = new FakeMediator((message, _) =>
        {
            Assert.IsType<UpdateCaptureCommand>(message);
            return Task.FromResult<object?>(null);
        });

        var controller = new CapturesController(mediator, NullLogger<CapturesController>.Instance);

        var result = await controller.Update("missing", new UpdateCaptureRequest { Text = "Updated" });

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDeleted()
    {
        var mediator = new FakeMediator((message, _) =>
        {
            var command = Assert.IsType<DeleteCaptureCommand>(message);
            Assert.Equal("capture-1", command.Id);
            return Task.FromResult<object?>(true);
        });

        var controller = new CapturesController(mediator, NullLogger<CapturesController>.Instance);

        var result = await controller.Delete("capture-1");

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenNotDeleted()
    {
        var mediator = new FakeMediator((message, _) =>
        {
            Assert.IsType<DeleteCaptureCommand>(message);
            return Task.FromResult<object?>(false);
        });

        var controller = new CapturesController(mediator, NullLogger<CapturesController>.Instance);

        var result = await controller.Delete("missing");

        Assert.IsType<NotFoundResult>(result);
    }
}
