using FluentValidation;
using ProductControl.Application.Commands;

namespace ProductControl.Application.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Product description is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.IsAvailable)
            .Must(value => value == true || value == false)
            .WithMessage("IsAvailable must be a boolean value.");
    }
}