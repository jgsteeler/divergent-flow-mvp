using DivergentFlow.Services.Features.Captures.Commands;
using FluentValidation;

namespace DivergentFlow.Services.Features.Captures.Validation;

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
