using AutoMapper;
using MediatR;
using ProductControl.Application.Commands;
using ProductControl.Application.DTOs;
using ProductControl.Core.Entities;
using ProductControl.Core.Interfaces;

namespace ProductControl.Application.Handlers;

public class CreateProductCommandHandler : BaseHandler, IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            IsAvailable = request.IsAvailable,
            UserId = tokenService.GetUserIdFromToken(request.User),
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.AddAsync(product);
        return _mapper.Map<ProductDto>(product);
    }
}