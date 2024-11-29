using MediatR;
using UserControl.Application.DTOs;

namespace UserControl.Application.Commands.UsersControllerCommands;

public class GetAllUsersCommand : IRequest<IEnumerable<UserDto>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }

    public GetAllUsersCommand(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
}