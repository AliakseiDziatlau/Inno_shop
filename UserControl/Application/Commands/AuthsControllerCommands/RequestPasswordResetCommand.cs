using MediatR;

namespace UserControl.Application.Commands.AuthsControllerCommands;

public class RequestPasswordResetCommand : IRequest<Unit>
{
    public string Email { get; set; } = string.Empty;
}