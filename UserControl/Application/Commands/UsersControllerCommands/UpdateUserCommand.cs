using MediatR;

namespace UserControl.Application.Commands.UsersControllerCommands;

public class UpdateUserCommand : IRequest<Unit>
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public UpdateUserCommand(int userId, string name, string email, string role)
    {
        UserId = userId;
        Name = name;
        Email = email;
        Role = role;
    }
}