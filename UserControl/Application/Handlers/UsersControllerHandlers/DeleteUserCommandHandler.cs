using MediatR;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.Interfaces;

namespace UserControl.Application.Handlers.UsersControllerHandlers;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserService _userService;

    public DeleteUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await _userService.DeleteUserAsync(request.UserId);
        return Unit.Value; 
    }
}