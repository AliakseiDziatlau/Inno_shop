using MediatR;

namespace UserControl.Application.Commands.AuthsControllerCommands;

public class ConfirmEmailCommand : IRequest<Unit>
{
    public string Token { get; set; }

    public ConfirmEmailCommand(string token)
    {
        Token = token;
    }
}