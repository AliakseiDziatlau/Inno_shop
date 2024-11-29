using MediatR;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.Interfaces;

namespace UserControl.Application.Handlers.UsersControllerHandlers;

public class DeactivateUserCommandHandler : UserHandlerBase, IRequestHandler<DeactivateUserCommand, Unit>
{
    public DeactivateUserCommandHandler(IUserService userService) : base(userService) { }

    public async Task<Unit> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        await _userService.DeactivateUserAsync(request.UserId);
        return Unit.Value;
    }
}