using MediatR;
using UserControl.Application.DTOs;

namespace UserControl.Application.Commands.UsersControllerCommands;

public class GetUserByIdCommand : IRequest<UserDto>
{
    public int UserId { get; }

    public GetUserByIdCommand(int userId)
    {
        UserId = userId;
    }
}