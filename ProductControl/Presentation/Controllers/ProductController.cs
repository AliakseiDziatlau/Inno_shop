using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductControl.Application.Commands;
using ProductControl.Application.DTOs;
namespace ProductControl.Presentation.Controllers;

/*
   Only owner of the product has access to it and can perform CRUD operations on it
 */
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var query = new GetProductByIdCommand(id, User);
        var productDto = await _mediator.Send(query);
        return Ok(productDto); 
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        command.User = User;
        var createdProduct = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetProducts([FromQuery] ProductFilterDto filterDto)
    {
        var query = new GetProductsCommand { FilterDto = filterDto, User = User };
        var products = await _mediator.Send(query);
        return Ok(products); 
    }
    
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
    {
        command.ProductId = id;
        command.User = User;
        await _mediator.Send(command);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var command = new DeleteProductCommand { ProductId = id, User = User };
        await _mediator.Send(command);
        return NoContent();
    }
   
    /*
     Performs Soft Delete for products
     */
    [Authorize(Roles = "Admin")]
    [HttpPut("toggle-user-products/{userId}")]
    public async Task<IActionResult> ToggleUserProducts(int userId, [FromBody] bool isActive)
    {
        var command = new ToggleUserProductsCommand { UserId = userId, IsActive = isActive };
        await _mediator.Send(command);
        return NoContent();
    }
    
    /*
     Performs Hard Delete for products
     */
    [Authorize(Roles = "Admin")]
    [HttpDelete("user/{userId}")]
    public async Task<IActionResult> DeleteProductsByUser(int userId)
    {
        var command = new DeleteProductsByUserCommand(userId);
        await _mediator.Send(command);
        return NoContent();
    }   
}