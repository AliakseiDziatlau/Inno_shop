using MediatR;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.Interfaces;

namespace UserControl.Application.Handlers.UsersControllerHandlers;

public class DeleteUserCommandHandler : UserHandlerBase, IRequestHandler<DeleteUserCommand, Unit>
{
    public DeleteUserCommandHandler(IUserService userService) : base(userService) { }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await _userService.DeleteUserAsync(request.UserId);
        return Unit.Value; 
    }
}