using FluentValidation;
using UserControl.Application.Commands.AuthsControllerCommands;

namespace UserControl.Application.Validators.AuthsControllerValidators;

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.")
            .MinimumLength(10).WithMessage("Token is invalid.");
    }
}