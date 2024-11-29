using FluentValidation.TestHelper;
using ProductControl.Application.Commands;
using ProductControl.Application.Validators;

namespace UserControlTests.ProductControl.UnitTests.ValidatorsTests;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        var command = new CreateProductCommand { Name = "", Description = "A great product", Price = 10, IsAvailable = true };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Product name is required.");
    }

    [Fact]
    public void Should_HaveError_When_DescriptionIsEmpty()
    {
        var command = new CreateProductCommand { Name = "Product 1", Description = "", Price = 10, IsAvailable = true };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Product description is required.");
    }

    [Fact]
    public void Should_HaveError_When_PriceIsZeroOrNegative()
    {
        var commandWithZeroPrice = new CreateProductCommand { Name = "Product 1", Description = "A great product", Price = 0, IsAvailable = true };
        var commandWithNegativePrice = new CreateProductCommand { Name = "Product 1", Description = "A great product", Price = -1, IsAvailable = true };
        var resultZeroPrice = _validator.TestValidate(commandWithZeroPrice);
        var resultNegativePrice = _validator.TestValidate(commandWithNegativePrice);
        resultZeroPrice.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("Price must be greater than zero.");
        resultNegativePrice.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("Price must be greater than zero.");
    }

    [Fact]
    public void Should_NotHaveError_When_AllPropertiesAreValid()
    {
        var command = new CreateProductCommand { Name = "Product 1", Description = "A great product", Price = 10, IsAvailable = true };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
        result.ShouldNotHaveValidationErrorFor(x => x.IsAvailable);
    }
}