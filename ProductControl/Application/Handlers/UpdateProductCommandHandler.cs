using AutoMapper;
using MediatR;
using ProductControl.Application.Commands;
using ProductControl.Core.Interfaces;
using ProductControl.Infrastracture.Interfaces;
namespace ProductControl.Application.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IProductRepository _repository;
    private readonly ITokenService _tokenService;

    public UpdateProductCommandHandler(IProductRepository repository, ITokenService tokenService)
    {
        _repository = repository;
        _tokenService = tokenService;
    }

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var userId = _tokenService.GetUserIdFromToken(request.User);
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