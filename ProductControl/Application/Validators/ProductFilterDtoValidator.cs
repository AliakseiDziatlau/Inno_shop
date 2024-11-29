using FluentValidation;
using ProductControl.Application.DTOs;
namespace ProductControl.Application.Validators;

public class ProductFilterDtoValidator : AbstractValidator<ProductFilterDto>
{
    public ProductFilterDtoValidator()
    {
        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum price must be greater than or equal to zero.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Maximum price must be greater than or equal to zero.")
            .GreaterThanOrEqualTo(x => x.MinPrice)
            .WithMessage("Maximum price must be greater than or equal to minimum price.");

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("Product name must be at most 100 characters.");
    }
}