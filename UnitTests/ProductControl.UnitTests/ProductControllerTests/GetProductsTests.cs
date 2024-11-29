using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductControl.Application.Commands;
using ProductControl.Application.DTOs;

namespace UserControlTests.ProductControl.UnitTests.ProductControllerTests;

public class GetProductsTests : ProductsControllerTestsBase
{
    [Fact]
    public async Task GetProducts_ReturnsOkResultWithProducts_WhenUserHasAccess()
    {
        var filterDto = new ProductFilterDto
        {
            Name = "Test Product",
            MinPrice = 50,
            MaxPrice = 150,
            IsAvailable = true
        };

        var expectedProducts = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1", Description = "Description 1", Price = 100, IsAvailable = true },
            new ProductDto { Id = 2, Name = "Product 2", Description = "Description 2", Price = 120, IsAvailable = true }
        };

        MediatorMock.Setup(m => m.Send(It.Is<GetProductsCommand>(q => 
            q.FilterDto == filterDto && q.User == Controller.User), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProducts);
        
        var result = await Controller.GetProducts(filterDto);
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsType<List<ProductDto>>(okResult.Value);

        Assert.Equal(expectedProducts.Count, returnedProducts.Count);
        Assert.Equal(expectedProducts, returnedProducts);

        MediatorMock.Verify(m => m.Send(It.Is<GetProductsCommand>(q => 
            q.FilterDto == filterDto && q.User == Controller.User), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProducts_ReturnsOkResultWithEmptyList_WhenNoProductsExistForUser()
    {
        var filterDto = new ProductFilterDto
        {
            Name = "Nonexistent Product",
            MinPrice = 500,
            MaxPrice = 1000,
            IsAvailable = false
        };

        MediatorMock.Setup(m => m.Send(It.Is<GetProductsCommand>(q => 
            q.FilterDto == filterDto && q.User == Controller.User), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductDto>());
        
        var result = await Controller.GetProducts(filterDto);
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsType<List<ProductDto>>(okResult.Value);

        Assert.Empty(returnedProducts);

        MediatorMock.Verify(m => m.Send(It.Is<GetProductsCommand>(q => 
            q.FilterDto == filterDto && q.User == Controller.User), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProducts_ThrowsUnauthorizedAccessException_WhenUserHasNoAccess()
    {
        var filterDto = new ProductFilterDto
        {
            Name = "Test Product"
        };

        MediatorMock.Setup(m => m.Send(It.Is<GetProductsCommand>(q => 
            q.FilterDto == filterDto && q.User == Controller.User), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("You are not authorized to access these products."));
        
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.GetProducts(filterDto));
        Assert.Equal("You are not authorized to access these products.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<GetProductsCommand>(q => 
            q.FilterDto == filterDto && q.User == Controller.User), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProducts_ReturnsEmptyList_WhenFilterMatchesNoProducts()
    {
        var filterDto = new ProductFilterDto
        {
            MinPrice = 500,
            MaxPrice = 1000,
            IsAvailable = false
        };

        MediatorMock.Setup(m => m.Send(It.Is<GetProductsCommand>(q => 
            q.FilterDto == filterDto && q.User == Controller.User), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductDto>());
        
        var result = await Controller.GetProducts(filterDto);
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsType<List<ProductDto>>(okResult.Value);

        Assert.Empty(returnedProducts);

        MediatorMock.Verify(m => m.Send(It.Is<GetProductsCommand>(q => 
            q.FilterDto == filterDto && q.User == Controller.User), It.IsAny<CancellationToken>()), Times.Once);
    }
}
