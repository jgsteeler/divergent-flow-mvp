using DivergentFlow.Application.Features.TypeInference.Commands;
using FluentValidation;

namespace DivergentFlow.Application.Features.TypeInference.Validation;

public sealed class ConfirmTypeCommandValidator : AbstractValidator<ConfirmTypeCommand>
{
    public ConfirmTypeCommandValidator()
    {
        RuleFor(x => x.Request)
            .NotNull();

        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request.Text).NotEmpty();
            RuleFor(x => x.Request.InferredType).NotEmpty();
            RuleFor(x => x.Request.ConfirmedType).NotEmpty();
            RuleFor(x => x.Request.InferredConfidence).InclusiveBetween(0, 100);
        });
    }
}
