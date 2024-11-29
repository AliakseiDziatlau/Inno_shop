using AutoMapper;
using MediatR;
using ProductControl.Application.DTOs;
using ProductControl.Core.Interfaces;
namespace ProductControl.Application.Handlers;

public class GetProductsCommand : ProductHandlerBase, IRequestHandler<Commands.GetProductsCommand, List<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductsCommand(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> Handle(Commands.GetProductsCommand request, CancellationToken cancellationToken)
    {
        var userId = tokenService.GetUserIdFromToken(request.User);
        var products = await _repository.GetByFilterAsync(request.FilterDto, userId);
        return _mapper.Map<List<ProductDto>>(products);
    }
}