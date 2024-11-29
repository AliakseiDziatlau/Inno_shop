using MediatR;
using ProductControl.Application.Commands;
using ProductControl.Core.Interfaces;

namespace ProductControl.Application.Handlers;

public class DeleteProductsByUserCommandHandler : IRequestHandler<DeleteProductsByUserCommand, Unit>
{
    private readonly IProductRepository _repository;

    public DeleteProductsByUserCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteProductsByUserCommand request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllByUserIdAsync(request.UserId, includeDeleted: false);
        _repository.RemoveRange(products);
        await _repository.SaveChangesAsync();
        return Unit.Value;
    }
}