using MediatR;

namespace ProductControl.Application.Commands;

public class ToggleUserProductsCommand : BaseCommand<Unit>
{
    public int UserId { get; set; }
    public bool IsActive { get; set; }
}