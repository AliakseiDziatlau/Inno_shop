using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductControl.Application.Commands;
using ProductControl.Application.DTOs;

namespace UserControlTests.ProductControl.UnitTests.ProductControllerTests;

public class GetProductByIdTests : ProductsControllerTestsBase
{
    [Fact]
    public async Task GetProductById_ReturnsOkResult_WhenProductExistsAndUserHasAccess()
    {
        var userId = 1; 
        var productId = 100; 
        var productDto = new ProductDto
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description",
            Price = 50.0m,
            IsAvailable = true
        };
        SetupUserInController(userId);
        MediatorMock.Setup(m => m.Send(It.Is<GetProductByIdCommand>(c => c.ProductId == productId && c.User != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync(productDto);
        var result = await Controller.GetProductById(productId);
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(productDto.Id, returnedProduct.Id);
        Assert.Equal(productDto.Name, returnedProduct.Name);
        Assert.Equal(productDto.Description, returnedProduct.Description);
        Assert.Equal(productDto.Price, returnedProduct.Price);
        Assert.Equal(productDto.IsAvailable, returnedProduct.IsAvailable);
        MediatorMock.Verify(m => m.Send(It.Is<GetProductByIdCommand>(c => c.ProductId == productId && c.User != null), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var productId = 100;

        SetupUserInController(1); 

        MediatorMock.Setup(m => m.Send(It.Is<GetProductByIdCommand>(c => c.ProductId == productId && c.User != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDto)null);

        var result = await Controller.GetProductById(productId);

        var notFoundResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(null, notFoundResult.Value);

        MediatorMock.Verify(m => m.Send(It.Is<GetProductByIdCommand>(c => c.ProductId == productId && c.User != null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProductById_ReturnsForbidden_WhenUserHasNoAccess()
    {
        var productId = 100;
        SetupUserInController(1); 
        MediatorMock.Setup(m => m.Send(It.Is<GetProductByIdCommand>(c => c.ProductId == productId && c.User != null), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("You do not have access to this product."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.GetProductById(productId));

        Assert.Equal("You do not have access to this product.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<GetProductByIdCommand>(c => c.ProductId == productId && c.User != null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProductById_ThrowsArgumentException_WhenProductIdIsInvalid()
    {
        var invalidProductId = -100;

        SetupUserInController(1); 

        MediatorMock.Setup(m => m.Send(It.Is<GetProductByIdCommand>(c => c.ProductId == invalidProductId && c.User != null), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Product ID must be greater than 0."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.GetProductById(invalidProductId));

        Assert.Equal("Product ID must be greater than 0.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<GetProductByIdCommand>(c => c.ProductId == invalidProductId && c.User != null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProductById_VerifiesCorrectServiceCalls()
    {
        var productId = 100;
        var productDto = new ProductDto
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description",
            Price = 50.0m,
            IsAvailable = true
        };

        SetupUserInController(1);

        MediatorMock.Setup(m => m.Send(It.Is<GetProductByIdCommand>(c => c.ProductId == productId && c.User != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync(productDto);

        await Controller.GetProductById(productId);

        MediatorMock.Verify(m => m.Send(It.Is<GetProductByIdCommand>(c => c.ProductId == productId && c.User != null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProductById_ThrowsException_WhenUnexpectedErrorOccurs()
    {
        var productId = 100;

        SetupUserInController(1); 

        MediatorMock.Setup(m => m.Send(It.Is<GetProductByIdCommand>(c => c.ProductId == productId && c.User != null), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error occurred."));

        var exception = await Assert.ThrowsAsync<Exception>(() => Controller.GetProductById(productId));

        Assert.Equal("Unexpected error occurred.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<GetProductByIdCommand>(c => c.ProductId == productId && c.User != null), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    protected void SetupUserInController(int userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        Controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }
}