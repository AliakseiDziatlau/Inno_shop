using MediatR;

namespace UserControl.Application.Commands.AuthsControllerCommands;

public class RegisterCommand : IRequest<Unit>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}