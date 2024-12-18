using MediatR;
using UserControl.Application.Commands.AuthsControllerCommands;
using UserControl.Application.DTOs;
using UserControl.Application.Interfaces;
namespace UserControl.Application.Handlers.AuthsControllerHandlers;

public class ResetPasswordCommandHandler : AuthHandlerBase, IRequestHandler<ResetPasswordCommand, Unit>
{
    public ResetPasswordCommandHandler(IAuthService authService) : base(authService) { }

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        await _authService.ResetPasswordAsync(request.Token, new ResetPasswordRequestDto
        {
            NewPassword = request.NewPassword
        });

        return Unit.Value;
    }
}