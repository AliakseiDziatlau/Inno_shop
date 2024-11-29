using MediatR;

namespace ProductControl.Application.Commands;

public class DeleteProductCommand : BaseCommand<Unit>
{
    public int ProductId { get; set; }
}