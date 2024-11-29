using MediatR;

namespace UserControl.Application.Commands.UsersControllerCommands;

public class DeleteUserCommand : IRequest<Unit>
{
    public int UserId { get; }

    public DeleteUserCommand(int userId)
    {
        UserId = userId;
    }
}