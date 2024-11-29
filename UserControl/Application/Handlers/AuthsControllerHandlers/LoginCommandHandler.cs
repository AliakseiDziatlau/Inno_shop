using MediatR;
using UserControl.Application.Commands.AuthsControllerCommands;
using UserControl.Application.DTOs;
using UserControl.Application.Interfaces;
namespace UserControl.Application.Handlers.AuthsControllerHandlers;

public class LoginCommandHandler : AuthHandlerBase, IRequestHandler<LoginCommand, LoginResponseDto>
{
    public LoginCommandHandler(IAuthService authService) : base(authService) { }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authService.LoginAsync(new LoginRequestDto
        {
            Email = request.Email,
            Password = request.Password
        });
    }
}