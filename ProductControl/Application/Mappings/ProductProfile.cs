using AutoMapper;
using ProductControl.Application.DTOs;
using ProductControl.Core.Entities;

namespace ProductControl.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>(); 
        CreateMap<ProductDto, Product>(); 
    }
}