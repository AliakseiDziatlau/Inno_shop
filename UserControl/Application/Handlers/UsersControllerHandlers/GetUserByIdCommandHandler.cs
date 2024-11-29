using MediatR;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.DTOs;
using UserControl.Application.Interfaces;
namespace UserControl.Application.Handlers.UsersControllerHandlers;

public class GetUserByIdCommandHandler : IRequestHandler<GetUserByIdCommand, UserDto>
{
    private readonly IUserService _userService;

    public GetUserByIdCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserDto> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(request.UserId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {request.UserId} not found.");
        }
        return user;
    }
}