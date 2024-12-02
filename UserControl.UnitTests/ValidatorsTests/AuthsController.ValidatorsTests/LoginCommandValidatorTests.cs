using FluentValidation.TestHelper;
using UserControl.Application.Commands.AuthsControllerCommands;
using UserControl.Application.Validators.AuthsControllerValidators;

namespace UserControl.UnitTests.ValidatorsTests.AuthsController.ValidatorsTests;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_EmailIsEmpty()
    {
        var command = new LoginCommand { Email = "", Password = "ValidPassword123" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Should_HaveError_When_EmailIsInvalid()
    {
        var command = new LoginCommand { Email = "invalid-email", Password = "ValidPassword123" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Should_NotHaveError_When_EmailIsValid()
    {
        var command = new LoginCommand { Email = "valid@example.com", Password = "ValidPassword123" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_HaveError_When_PasswordIsEmpty()
    {
        var command = new LoginCommand { Email = "valid@example.com", Password = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password)
              .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Should_NotHaveError_When_PasswordIsProvided()
    {
        var command = new LoginCommand { Email = "valid@example.com", Password = "ValidPassword123" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}