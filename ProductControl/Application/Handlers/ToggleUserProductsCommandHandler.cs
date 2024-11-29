using MediatR;
using ProductControl.Application.Commands;
using ProductControl.Core.Interfaces;
namespace ProductControl.Application.Handlers;

public class ToggleUserProductsCommandHandler : IRequestHandler<ToggleUserProductsCommand, Unit>
{
    private readonly IProductRepository _repository;

    public ToggleUserProductsCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(ToggleUserProductsCommand request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllByUserIdAsync(request.UserId, includeDeleted: true);
        foreach (var product in products)
        {
            product.IsDeleted = !request.IsActive;
        }
        await _repository.SaveChangesAsync();
        return Unit.Value;
    }
}