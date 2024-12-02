using FluentValidation.TestHelper;
using ProductControl.Application.Commands;
using ProductControl.Application.Validators;

namespace UserControlTests.ValidatorsTests;

public class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _validator;

    public UpdateProductCommandValidatorTests()
    {
        _validator = new UpdateProductCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        var command = new UpdateProductCommand { Name = "", Description = "A description", Price = 100, IsAvailable = true };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Product name is required.");
    }

    [Fact]
    public void Should_HaveError_When_NameIsTooLong()
    {
        var command = new UpdateProductCommand { Name = new string('a', 101), Description = "A description", Price = 100, IsAvailable = true };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Product name must not exceed 100 characters.");
    }

    [Fact]
    public void Should_HaveError_When_DescriptionIsEmpty()
    {
        var command = new UpdateProductCommand { Name = "Product", Description = "", Price = 100, IsAvailable = true };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Description is required.");
    }

    [Fact]
    public void Should_HaveError_When_DescriptionIsTooLong()
    { 
        var command = new UpdateProductCommand { Name = "Product", Description = new string('a', 501), Price = 100, IsAvailable = true };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Description must not exceed 500 characters.");
    }

    [Fact]
    public void Should_HaveError_When_PriceIsNegative()
    {
        var command = new UpdateProductCommand { Name = "Product", Description = "A description", Price = -1, IsAvailable = true };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Price)
              .WithErrorMessage("Price must be greater or equal than zero.");
    }

    [Fact]
    public void Should_NotHaveError_When_AllPropertiesAreValid()
    {
        var command = new UpdateProductCommand { Name = "Valid Product", Description = "Valid description", Price = 100, IsAvailable = true };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
        result.ShouldNotHaveValidationErrorFor(x => x.IsAvailable);
    }
}