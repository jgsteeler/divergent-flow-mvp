using DivergentFlow.Application.Features.Items.Commands;
using FluentValidation;

namespace DivergentFlow.Application.Features.Items.Validation;

public sealed class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Text)
            .NotEmpty();

        RuleFor(x => x.TypeConfidence)
            .InclusiveBetween(0, 100)
            .When(x => x.TypeConfidence is not null);
    }
}
