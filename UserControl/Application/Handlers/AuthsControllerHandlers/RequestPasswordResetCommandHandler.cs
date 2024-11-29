using MediatR;
using UserControl.Application.Commands.AuthsControllerCommands;
using UserControl.Application.DTOs;
using UserControl.Application.Interfaces;
namespace UserControl.Application.Handlers.AuthsControllerHandlers;

public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, Unit>
{
    private readonly IAuthService _authService;

    public RequestPasswordResetCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Unit> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        await _authService.RequestPasswordResetAsync(new PasswordResetRequestDto
        {
            Email = request.Email
        });

        return Unit.Value;
    }
}