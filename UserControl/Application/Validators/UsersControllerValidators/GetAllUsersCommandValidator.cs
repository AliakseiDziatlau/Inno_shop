using FluentValidation;
using UserControl.Application.Commands.UsersControllerCommands;

namespace UserControl.Application.Validators.UsersControllerValidators;

public class GetAllUsersCommandValidator : AbstractValidator<GetAllUsersCommand>
{
    public GetAllUsersCommandValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");
    }
}