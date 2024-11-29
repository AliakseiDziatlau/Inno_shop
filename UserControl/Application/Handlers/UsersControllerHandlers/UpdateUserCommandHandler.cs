using MediatR;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.DTOs;
using UserControl.Application.Interfaces;
namespace UserControl.Application.Handlers.UsersControllerHandlers;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = new UpdateUserRequestDto
        {
            Name = request.Name,
            Email = request.Email,
            Role = request.Role
        };
        await _userService.UpdateUserAsync(request.UserId, updateRequest);
        return Unit.Value; 
    }
}