using MediatR;

namespace UserControl.Application.Commands.AuthsControllerCommands;

public class ResetPasswordCommand : IRequest<Unit>
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}