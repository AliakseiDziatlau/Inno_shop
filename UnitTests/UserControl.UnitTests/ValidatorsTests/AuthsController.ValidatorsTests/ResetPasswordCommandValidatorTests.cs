using FluentValidation.TestHelper;
using UserControl.Application.Commands.AuthsControllerCommands;
using UserControl.Application.Validators.AuthsControllerValidators;

namespace UserControlTests.UserControl.UnitTests.ValidatorsTests.AuthsController.ValidatorsTests;

public class ResetPasswordCommandValidatorTests
{
    private readonly ResetPasswordCommandValidator _validator;

    public ResetPasswordCommandValidatorTests()
    {
        _validator = new ResetPasswordCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_TokenIsEmpty()
    {
        var command = new ResetPasswordCommand { Token = "", NewPassword = "ValidPassword123" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Token)
              .WithErrorMessage("Token is required.");
    }

    [Fact]
    public void Should_NotHaveError_When_TokenIsProvided()
    {
        var command = new ResetPasswordCommand { Token = "valid-token", NewPassword = "ValidPassword123" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Token);
    }

    [Fact]
    public void Should_HaveError_When_NewPasswordIsEmpty()
    {
        var command = new ResetPasswordCommand { Token = "valid-token", NewPassword = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
              .WithErrorMessage("New password is required.");
    }

    [Fact]
    public void Should_HaveError_When_NewPasswordIsTooShort()
    {
        var command = new ResetPasswordCommand { Token = "valid-token", NewPassword = "short" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
              .WithErrorMessage("New password must be at least 8 characters long.");
    }

    [Fact]
    public void Should_NotHaveError_When_NewPasswordIsValid()
    {
        var command = new ResetPasswordCommand { Token = "valid-token", NewPassword = "ValidPassword123" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }
}