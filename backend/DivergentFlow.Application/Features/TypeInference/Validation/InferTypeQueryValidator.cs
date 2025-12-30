using DivergentFlow.Application.Features.TypeInference.Queries;
using FluentValidation;

namespace DivergentFlow.Application.Features.TypeInference.Validation;

public sealed class InferTypeQueryValidator : AbstractValidator<InferTypeQuery>
{
    public InferTypeQueryValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty();
    }
}
