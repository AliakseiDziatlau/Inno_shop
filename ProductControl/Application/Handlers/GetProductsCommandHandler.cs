using AutoMapper;
using MediatR;
using ProductControl.Application.Commands;
using ProductControl.Application.DTOs;
using ProductControl.Core.Interfaces;
namespace ProductControl.Application.Handlers;

public class GetProductsCommandHandler : BaseHandler, IRequestHandler<GetProductsCommand, List<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductsCommandHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> Handle(GetProductsCommand request, CancellationToken cancellationToken)
    {
        var userId = tokenService.GetUserIdFromToken(request.User);
        var products = await _repository.GetByFilterAsync(request.FilterDto, userId);
        return _mapper.Map<List<ProductDto>>(products);
    }
}