using MediatR;
using ProductControl.Application.Commands;
using ProductControl.Core.Interfaces;
using ProductControl.Infrastracture.Interfaces;
namespace ProductControl.Application.Handlers;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductRepository _repository;
    private readonly ITokenService _tokenService;

    public DeleteProductCommandHandler(IProductRepository repository, ITokenService tokenService)
    {
        _repository = repository;
        _tokenService = tokenService;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var userId = _tokenService.GetUserIdFromToken(request.User);
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