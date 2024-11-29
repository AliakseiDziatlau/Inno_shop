using FluentValidation.TestHelper;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.Validators.UsersControllerValidators;

namespace UserControlTests.UserControl.UnitTests.ValidatorsTests.UsersController.ValidatorsTests;

public class GetAllUsersCommandValidatorTests
{
    private readonly GetAllUsersCommandValidator _validator;

    public GetAllUsersCommandValidatorTests()
    {
        _validator = new GetAllUsersCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_PageIsLessThanOrEqualToZero()
    {
        var command = new GetAllUsersCommand(0, 10);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorMessage("Page must be greater than 0.");
    }

    [Fact]
    public void Should_HaveError_When_PageSizeIsLessThan1()
    {
        var command = new GetAllUsersCommand(1, 0);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must be between 1 and 100.");
    }

    [Fact]
    public void Should_HaveError_When_PageSizeIsGreaterThan100()
    {
        var command = new GetAllUsersCommand(1,101);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must be between 1 and 100.");
    }

    [Fact]
    public void Should_NotHaveError_When_PageIsGreaterThanZero_And_PageSizeIsBetween1And100()
    {
        var command = new GetAllUsersCommand(1,50);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Page);
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }
}