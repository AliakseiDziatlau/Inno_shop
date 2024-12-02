using FluentValidation.TestHelper;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.Validators.UsersControllerValidators;

namespace UserControl.UnitTests.ValidatorsTests.UsersController.ValidatorsTests;

public class UpdateUserCommandValidatorTests
{
    private readonly UpdateUserCommandValidator _validator;

    public UpdateUserCommandValidatorTests()
    {
        _validator = new UpdateUserCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_UserIdIsZeroOrNegative()
    {
        var command = new UpdateUserCommand(0, "John","john@example.com","User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
              .WithErrorMessage("UserId must be a positive integer.");
    }

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        var command = new UpdateUserCommand(1,"","john@example.com","User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name is required.");
    }

    [Fact]
    public void Should_HaveError_When_NameExceedsMaxLength()
    {
        var command = new UpdateUserCommand(1, new string('A', 51),"john@example.com","User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name must not exceed 50 characters.");
    }

    [Fact]
    public void Should_HaveError_When_EmailIsEmpty()
    {
        var command = new UpdateUserCommand (1,"John","", "User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Should_HaveError_When_EmailIsInvalid()
    {
        var command = new UpdateUserCommand(1, "John","invalid-email","User");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Should_HaveError_When_RoleIsEmpty()
    {
        var command = new UpdateUserCommand(1,"John", "john@example.com","");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role)
              .WithErrorMessage("Role is required.");
    }

    [Fact]
    public void Should_HaveError_When_RoleIsInvalid()
    {
        var command = new UpdateUserCommand(1, "John", "john@example.com", "Manager");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Role)
              .WithErrorMessage("Role must be either 'Admin' or 'User'.");
    }

    [Fact]
    public void Should_NotHaveError_When_ValidDataIsProvided()
    {
        var command = new UpdateUserCommand(1, "John", "john@example.com","User");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }
}