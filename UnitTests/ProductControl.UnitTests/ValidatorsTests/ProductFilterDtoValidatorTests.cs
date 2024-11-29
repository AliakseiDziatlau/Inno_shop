using FluentValidation.TestHelper;
using ProductControl.Application.DTOs;
using ProductControl.Application.Validators;

namespace UserControlTests.ProductControl.UnitTests.ValidatorsTests;

public class ProductFilterDtoValidatorTests
{
    private readonly ProductFilterDtoValidator _validator;

    public ProductFilterDtoValidatorTests()
    {
        _validator = new ProductFilterDtoValidator();
    }

    [Fact]
    public void Should_HaveError_When_MinPriceIsNegative()
    {
        var filterDto = new ProductFilterDto { MinPrice = -1, MaxPrice = 100, Name = "Product" };
        var result = _validator.TestValidate(filterDto);
        result.ShouldHaveValidationErrorFor(x => x.MinPrice)
              .WithErrorMessage("Minimum price must be greater than or equal to zero.");
    }

    [Fact]
    public void Should_HaveError_When_MaxPriceIsNegative()
    {
        var filterDto = new ProductFilterDto { MinPrice = 0, MaxPrice = -1, Name = "Product" };
        var result = _validator.TestValidate(filterDto);
        result.ShouldHaveValidationErrorFor(x => x.MaxPrice)
              .WithErrorMessage("Maximum price must be greater than or equal to zero.");
    }

    [Fact]
    public void Should_HaveError_When_MaxPriceIsLessThanMinPrice()
    {
        var filterDto = new ProductFilterDto { MinPrice = 100, MaxPrice = 50, Name = "Product" };
        var result = _validator.TestValidate(filterDto);
        result.ShouldHaveValidationErrorFor(x => x.MaxPrice)
              .WithErrorMessage("Maximum price must be greater than or equal to minimum price.");
    }

    [Fact]
    public void Should_HaveError_When_NameIsLongerThan100Characters()
    {
        var filterDto = new ProductFilterDto { MinPrice = 0, MaxPrice = 100, Name = new string('a', 101) };
        var result = _validator.TestValidate(filterDto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Product name must be at most 100 characters.");
    }

    [Fact]
    public void Should_NotHaveError_When_AllPropertiesAreValid()
    {
        var filterDto = new ProductFilterDto { MinPrice = 0, MaxPrice = 100, Name = "Product" };
        var result = _validator.TestValidate(filterDto);
        result.ShouldNotHaveValidationErrorFor(x => x.MinPrice);
        result.ShouldNotHaveValidationErrorFor(x => x.MaxPrice);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}