using MediatR;

namespace UserControl.Application.Commands.UsersControllerCommands;

public class ActivateUserCommand : IRequest<Unit>
{
    public int UserId { get; set; }

    public ActivateUserCommand(int userId)
    {
        UserId = userId;
    }
}