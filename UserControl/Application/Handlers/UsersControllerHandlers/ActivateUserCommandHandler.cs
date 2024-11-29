using MediatR;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.Interfaces;

namespace UserControl.Application.Handlers.UsersControllerHandlers;

public class ActivateUserCommandHandler : UserHandlerBase, IRequestHandler<ActivateUserCommand, Unit>
{
    public ActivateUserCommandHandler(IUserService userService) : base(userService) { }
    
    public async Task<Unit> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        await _userService.ActivateUserAsync(request.UserId);
        return Unit.Value;
    }
}