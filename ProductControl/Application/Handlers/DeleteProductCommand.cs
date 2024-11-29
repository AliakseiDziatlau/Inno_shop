using MediatR;
using ProductControl.Core.Interfaces;
namespace ProductControl.Application.Handlers;

public class DeleteProductCommand : ProductHandlerBase, IRequestHandler<Commands.DeleteProductCommand, Unit>
{
    private readonly IProductRepository _repository;

    public DeleteProductCommand(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(Commands.DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var userId = tokenService.GetUserIdFromToken(request.User);
        var product = await _repository.GetByIdAsync(request.ProductId, userId);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found or you don't have access to delete it.");
        }
        
        await _repository.DeleteAsync(product);
        await _repository.SaveChangesAsync();
        return Unit.Value;
    }
}