using System.Security.Claims;
using ProductControl.Application.DTOs;
namespace ProductControl.Application.Commands;

public class GetProductByIdCommand : BaseCommand<ProductDto>
{
    public int ProductId { get; set; }

    public GetProductByIdCommand(int productId, ClaimsPrincipal user)
    {
        ProductId = productId;
        User = user;
    }
}