using MediatR;
using ProductControl.Core.Interfaces;
namespace ProductControl.Application.Handlers;

public class UpdateProductCommand : ProductHandlerBase, IRequestHandler<Commands.UpdateProductCommand, Unit>
{
    private readonly IProductRepository _repository;

    public UpdateProductCommand(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(Commands.UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var userId = tokenService.GetUserIdFromToken(request.User);
        var product = await _repository.GetByIdAsync(request.ProductId, userId);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found or you don't have access to this product.");
        }
        
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.IsAvailable = request.IsAvailable;

        await _repository.SaveChangesAsync();
        return Unit.Value;
    }
}