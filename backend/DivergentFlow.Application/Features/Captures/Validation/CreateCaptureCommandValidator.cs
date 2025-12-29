using DivergentFlow.Application.Features.Captures.Commands;
using FluentValidation;

namespace DivergentFlow.Application.Features.Captures.Validation;

public sealed class CreateCaptureCommandValidator : AbstractValidator<CreateCaptureCommand>
{
    public CreateCaptureCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty();

        RuleFor(x => x.TypeConfidence)
            .InclusiveBetween(0, 100)
            .When(x => x.TypeConfidence is not null);
    }
}
