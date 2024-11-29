using MediatR;

namespace ProductControl.Application.Commands;

public class DeleteProductsByUserCommand : BaseCommand<Unit>
{
    public int UserId { get; set; }

    public DeleteProductsByUserCommand(int userId)
    {
        UserId = userId;
    }
}