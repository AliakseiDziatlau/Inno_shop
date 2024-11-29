using MediatR;

namespace UserControl.Application.Commands.UsersControllerCommands;

public class DeactivateUserCommand : IRequest<Unit>
{
    public int UserId { get; set; }

    public DeactivateUserCommand(int userId)
    {
        UserId = userId;
    }
}