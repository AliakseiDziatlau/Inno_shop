using FluentValidation;
using UserControl.Application.Commands.UsersControllerCommands;

namespace UserControl.Application.Validators.UsersControllerValidators;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(command => command.UserId)
            .GreaterThan(0)
            .WithMessage("UserId must be a positive integer.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters.");

        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(command => command.Role)
            .NotEmpty()
            .WithMessage("Role is required.")
            .Must(role => role == "Admin" || role == "User")
            .WithMessage("Role must be either 'Admin' or 'User'.");
    }
}