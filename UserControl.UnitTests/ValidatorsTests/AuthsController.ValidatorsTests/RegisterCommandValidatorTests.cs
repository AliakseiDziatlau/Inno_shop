using FluentValidation.TestHelper;
using UserControl.Application.Commands.AuthsControllerCommands;
using UserControl.Application.Validators.AuthsControllerValidators;

namespace UserControl.UnitTests.ValidatorsTests.AuthsController.ValidatorsTests;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        var command = new RegisterCommand { Name = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name is required.");
    }

    [Fact]
    public void Should_HaveError_When_NameExceedsMaximumLength()
    {
        var command = new RegisterCommand { Name = new string('a', 51) };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name must not exceed 50 characters.");
    }

    [Fact]
    public void Should_HaveError_When_EmailIsEmpty()
    {
        var command = new RegisterCommand { Email = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Should_HaveError_When_EmailIsInvalid()
    {
        var command = new RegisterCommand { Email = "invalid-email" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Should_HaveError_When_PasswordIsEmpty()
    {
        var command = new RegisterCommand { Password = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password)
              .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Should_HaveError_When_PasswordIsTooShort()
    {
        var command = new RegisterCommand { Password = "1234567" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password)
              .WithErrorMessage("Password must be at least 8 characters long.");
    }

    [Fact]
    public void Should_HaveError_When_RoleIsEmpty()
    {
        var command = new RegisterCommand { Role = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role)
              .WithErrorMessage("Role is required.");
    }

    [Fact]
    public void Should_HaveError_When_RoleIsInvalid()
    {
        var command = new RegisterCommand { Role = "InvalidRole" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role)
              .WithErrorMessage("Role must be either 'User' or 'Admin'.");
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        var command = new RegisterCommand
        {
            Name = "Valid Name",
            Email = "valid@example.com",
            Password = "ValidPassword123",
            Role = "User"
        };

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }
}