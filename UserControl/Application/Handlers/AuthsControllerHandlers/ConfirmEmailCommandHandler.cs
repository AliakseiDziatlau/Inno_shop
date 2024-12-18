using MediatR;
using UserControl.Application.Commands.AuthsControllerCommands;
using UserControl.Application.Interfaces;
namespace UserControl.Application.Handlers.AuthsControllerHandlers;

public class ConfirmEmailCommandHandler : AuthHandlerBase, IRequestHandler<ConfirmEmailCommand, Unit>
{
    public ConfirmEmailCommandHandler(IAuthService authService) : base(authService) { }

    public async Task<Unit> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        await _authService.ConfirmEmailAsync(request.Token);
        return Unit.Value;
    }
}