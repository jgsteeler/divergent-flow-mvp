using DivergentFlow.Api.Controllers;
using DivergentFlow.Api.Tests.TestDoubles;
using DivergentFlow.Application.Features.TypeInference.Commands;
using DivergentFlow.Application.Features.TypeInference.Queries;
using DivergentFlow.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DivergentFlow.Api.Tests;

public sealed class TypeControllerTests
{
    [Fact]
    public async Task Infer_ReturnsOk_WithTypeInferenceResult()
    {
        var request = new TypeInferenceRequest
        {
            Text = "Buy groceries tomorrow"
        };

        var expected = new TypeInferenceResult { InferredType = "action", Confidence = 50.0 };
        var mediator = new FakeMediator((message, _) =>
        {
            var query = Assert.IsType<InferTypeQuery>(message);
            Assert.Equal("Buy groceries tomorrow", query.Text);
            return Task.FromResult<object?>(expected);
        });
        var controller = new TypeController(mediator, NullLogger<TypeController>.Instance);

        var result = await controller.Infer(request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public async Task Infer_UsesEmptyString_WhenTextIsNull()
    {
        var request = new TypeInferenceRequest { Text = null! };

        var mediator = new FakeMediator((message, _) =>
        {
            var query = Assert.IsType<InferTypeQuery>(message);
            Assert.Equal(string.Empty, query.Text);
            return Task.FromResult<object?>(new TypeInferenceResult { InferredType = "action", Confidence = 50.0 });
        });
        var controller = new TypeController(mediator, NullLogger<TypeController>.Instance);

        var result = await controller.Infer(request);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Confirm_ReturnsNoContent()
    {
        var request = new TypeConfirmationRequest
        {
            Text = "Buy groceries",
            InferredType = "action",
            InferredConfidence = 50.0,
            ConfirmedType = "action"
        };

        var mediator = new FakeMediator((message, _) =>
        {
            var command = Assert.IsType<ConfirmTypeCommand>(message);
            Assert.Same(request, command.Request);
            return Task.FromResult<object?>(MediatR.Unit.Value);
        });
        var controller = new TypeController(mediator, NullLogger<TypeController>.Instance);

        var result = await controller.Confirm(request);

        Assert.IsType<NoContentResult>(result);
    }
}
