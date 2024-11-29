using MediatR;
using UserControl.Application.Commands.AuthsControllerCommands;
using UserControl.Application.DTOs;
using UserControl.Application.Interfaces;
namespace UserControl.Application.Handlers.AuthsControllerHandlers;

public class RequestPasswordResetCommandHandler : AuthHandlerBase, IRequestHandler<RequestPasswordResetCommand, Unit>
{
    public RequestPasswordResetCommandHandler(IAuthService authService) : base(authService) { }

    public async Task<Unit> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        await _authService.RequestPasswordResetAsync(new PasswordResetRequestDto
        {
            Email = request.Email
        });

        return Unit.Value;
    }
}