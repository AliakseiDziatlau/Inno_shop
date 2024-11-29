using AutoMapper;
using MediatR;
using ProductControl.Application.DTOs;
using ProductControl.Core.Interfaces;
namespace ProductControl.Application.Handlers;

public class GetProductByIdCommand : ProductHandlerBase, IRequestHandler<Commands.GetProductByIdCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductByIdCommand(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(Commands.GetProductByIdCommand request, CancellationToken cancellationToken)
    {
        var userId = tokenService.GetUserIdFromToken(request.User);
        var product = await _repository.GetByIdAsync(request.ProductId, userId);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found or you don't have access to this product.");
        }
        return _mapper.Map<ProductDto>(product);
    }
}