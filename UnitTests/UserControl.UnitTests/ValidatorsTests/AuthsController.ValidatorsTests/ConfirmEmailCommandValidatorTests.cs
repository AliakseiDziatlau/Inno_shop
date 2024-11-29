using FluentValidation.TestHelper;
using UserControl.Application.Commands.AuthsControllerCommands;
using UserControl.Application.Validators.AuthsControllerValidators;

namespace UserControlTests.UserControl.UnitTests.ValidatorsTests.AuthsController.ValidatorsTests;

public class ConfirmEmailCommandValidatorTests
{
    private readonly ConfirmEmailCommandValidator _validator;

    public ConfirmEmailCommandValidatorTests()
    {
        _validator = new ConfirmEmailCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_TokenIsEmpty()
    {
        var command = new ConfirmEmailCommand("");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Token)
            .WithErrorMessage("Token is required.");
    }

    [Fact]
    public void Should_HaveError_When_TokenIsTooShort()
    {
        var command = new ConfirmEmailCommand("short");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Token)
            .WithErrorMessage("Token is invalid.");
    }

    [Fact]
    public void Should_NotHaveError_When_TokenIsValid()
    {
        var command = new ConfirmEmailCommand( "validtoken123");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Token);
    }
}