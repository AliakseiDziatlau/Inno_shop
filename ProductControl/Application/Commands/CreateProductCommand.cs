using ProductControl.Application.DTOs;
namespace ProductControl.Application.Commands;

public class CreateProductCommand : BaseCommand<ProductDto>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}