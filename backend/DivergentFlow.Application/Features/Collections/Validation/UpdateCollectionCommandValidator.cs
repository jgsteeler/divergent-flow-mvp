using DivergentFlow.Application.Features.Collections.Commands;
using FluentValidation;

namespace DivergentFlow.Application.Features.Collections.Validation;

public sealed class UpdateCollectionCommandValidator : AbstractValidator<UpdateCollectionCommand>
{
    public UpdateCollectionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
