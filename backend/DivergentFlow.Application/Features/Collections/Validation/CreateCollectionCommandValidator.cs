using DivergentFlow.Application.Features.Collections.Commands;
using FluentValidation;

namespace DivergentFlow.Application.Features.Collections.Validation;

public sealed class CreateCollectionCommandValidator : AbstractValidator<CreateCollectionCommand>
{
    public CreateCollectionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
