using MediatR;
using UserControl.Application.Commands.AuthsControllerCommands;
using UserControl.Application.DTOs;
using UserControl.Application.Interfaces;

namespace UserControl.Application.Handlers.AuthsControllerHandlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var registerRequest = new RegisterRequestDto
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            Role = request.Role
        };

        await _authService.RegisterAsync(registerRequest);
        return Unit.Value;
    }
}