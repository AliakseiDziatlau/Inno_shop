using FluentValidation;
using UserControl.Application.Commands.AuthsControllerCommands;

namespace UserControl.Application.Validators.AuthsControllerValidators;

public class RequestPasswordResetCommandValidator : AbstractValidator<RequestPasswordResetCommand>
{
    public RequestPasswordResetCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}