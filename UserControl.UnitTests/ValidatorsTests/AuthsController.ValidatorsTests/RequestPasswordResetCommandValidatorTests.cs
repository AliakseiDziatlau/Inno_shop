using FluentValidation.TestHelper;
using UserControl.Application.Commands.AuthsControllerCommands;
using UserControl.Application.Validators.AuthsControllerValidators;

namespace UserControl.UnitTests.ValidatorsTests.AuthsController.ValidatorsTests;

public class RequestPasswordResetCommandValidatorTests
{
    private readonly RequestPasswordResetCommandValidator _validator;

    public RequestPasswordResetCommandValidatorTests()
    {
        _validator = new RequestPasswordResetCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_EmailIsEmpty()
    {
        var command = new RequestPasswordResetCommand { Email = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Should_HaveError_When_EmailIsInvalid()
    {
        var command = new RequestPasswordResetCommand { Email = "invalid-email" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Should_NotHaveError_When_EmailIsValid()
    {
        var command = new RequestPasswordResetCommand { Email = "valid@example.com" };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}