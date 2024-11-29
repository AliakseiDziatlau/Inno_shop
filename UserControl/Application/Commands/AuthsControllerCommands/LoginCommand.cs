using MediatR;
using UserControl.Application.DTOs;

namespace UserControl.Application.Commands.AuthsControllerCommands;

public class LoginCommand : IRequest<LoginResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}