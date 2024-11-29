using MediatR;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.Interfaces;

namespace UserControl.Application.Handlers.UsersControllerHandlers;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Unit>
{
    private readonly IUserService _userService;

    public ActivateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Unit> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        await _userService.ActivateUserAsync(request.UserId);
        return Unit.Value;
    }
}