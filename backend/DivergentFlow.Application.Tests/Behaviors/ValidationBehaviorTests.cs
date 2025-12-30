using DivergentFlow.Application.Behaviors;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Xunit;

namespace DivergentFlow.Application.Tests.Behaviors;

public sealed class ValidationBehaviorTests
{
    private sealed record TestRequest(string Value) : IRequest<string>;

    private sealed class TestValidator : AbstractValidator<TestRequest>
    {
        public TestValidator()
        {
            RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required");
        }
    }

    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
        // Arrange
        var validators = Array.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("test");
        var nextCalled = false;
        RequestHandlerDelegate<string> next = () =>
        {
            nextCalled = true;
            return Task.FromResult("success");
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("success", result);
    }

    [Fact]
    public async Task Handle_ValidRequest_CallsNext()
    {
        // Arrange
        var validators = new IValidator<TestRequest>[] { new TestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("test");
        var nextCalled = false;
        RequestHandlerDelegate<string> next = () =>
        {
            nextCalled = true;
            return Task.FromResult("success");
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("success", result);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var validators = new IValidator<TestRequest>[] { new TestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("");
        RequestHandlerDelegate<string> next = () => Task.FromResult("success");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(request, next, CancellationToken.None));

        Assert.NotEmpty(exception.Errors);
        Assert.Contains(exception.Errors, e => e.PropertyName == "Value" && e.ErrorMessage == "Value is required");
    }

    [Fact]
    public async Task Handle_MultipleValidators_AggregatesFailures()
    {
        // Arrange
        var validator1 = new TestValidator();
        var validator2 = new TestValidator2();
        var validators = new IValidator<TestRequest>[] { validator1, validator2 };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("ab");
        RequestHandlerDelegate<string> next = () => Task.FromResult("success");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(request, next, CancellationToken.None));

        Assert.Equal(2, exception.Errors.Count());
    }

    [Fact]
    public async Task Handle_MultipleValidators_AllValid_CallsNext()
    {
        // Arrange
        var validator1 = new TestValidator();
        var validator2 = new TestValidator2();
        var validators = new IValidator<TestRequest>[] { validator1, validator2 };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("test");
        var nextCalled = false;
        RequestHandlerDelegate<string> next = () =>
        {
            nextCalled = true;
            return Task.FromResult("success");
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("success", result);
    }

    [Fact]
    public async Task Handle_CancellationToken_PassedToValidators()
    {
        // Arrange
        var validator = new CancellationAwareValidator();
        var validators = new IValidator<TestRequest>[] { validator };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("test");
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        RequestHandlerDelegate<string> next = () => Task.FromResult("success");

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => behavior.Handle(request, next, cts.Token));
    }

    private sealed class TestValidator2 : AbstractValidator<TestRequest>
    {
        public TestValidator2()
        {
            RuleFor(x => x.Value).MinimumLength(3).WithMessage("Value must be at least 3 characters");
        }
    }

    private sealed class CancellationAwareValidator : AbstractValidator<TestRequest>
    {
        public CancellationAwareValidator()
        {
            RuleFor(x => x.Value).MustAsync(async (value, cancellation) =>
            {
                await Task.Delay(1, cancellation);
                return true;
            });
        }
    }
}
