using ProductControl.Application.DTOs;
namespace ProductControl.Application.Commands;

public class GetProductsCommand : BaseCommand<List<ProductDto>>
{
    public ProductFilterDto FilterDto { get; set; } = new ProductFilterDto();
}