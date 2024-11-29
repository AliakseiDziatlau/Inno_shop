using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductControl.Application.Commands;

namespace UserControlTests.ProductControl.UnitTests.ProductControllerTests;

public class UpdateProductTests : ProductsControllerTestsBase
{
    [Fact]
    public async Task UpdateProduct_ReturnsNoContent_WhenUpdateIsSuccessful()
    {
        var productId = 1;
        var updateCommand = new UpdateProductCommand
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 99.99m,
            IsAvailable = true,
            User = Controller.User 
        };

        MediatorMock.Setup(m => m.Send(It.Is<UpdateProductCommand>(c =>
            c.ProductId == productId &&
            c.Name == updateCommand.Name &&
            c.Description == updateCommand.Description &&
            c.Price == updateCommand.Price &&
            c.IsAvailable == updateCommand.IsAvailable &&
            c.User == Controller.User), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));
        
        var result = await Controller.UpdateProduct(productId, updateCommand);
        
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<UpdateProductCommand>(c =>
            c.ProductId == productId &&
            c.User == Controller.User), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ThrowsUnauthorizedAccessException_WhenUserIsNotOwner()
    {
        var productId = 1;
        var updateCommand = new UpdateProductCommand
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 99.99m,
            IsAvailable = true,
            User = Controller.User
        };

        MediatorMock.Setup(m => m.Send(It.Is<UpdateProductCommand>(c =>
            c.ProductId == productId && c.User == Controller.User), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("You do not have permission to update this product."));
        
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            Controller.UpdateProduct(productId, updateCommand));

        Assert.Equal("You do not have permission to update this product.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<UpdateProductCommand>(c =>
            c.ProductId == productId && c.User == Controller.User), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ThrowsKeyNotFoundException_WhenProductDoesNotExist()
    {
        var productId = 1;
        var updateCommand = new UpdateProductCommand
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 99.99m,
            IsAvailable = true,
            User = Controller.User
        };

        MediatorMock.Setup(m => m.Send(It.Is<UpdateProductCommand>(c =>
            c.ProductId == productId && c.User == Controller.User), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Product not found."));
        
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Controller.UpdateProduct(productId, updateCommand));

        Assert.Equal("Product not found.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<UpdateProductCommand>(c =>
            c.ProductId == productId && c.User == Controller.User), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ThrowsArgumentException_WhenRequestIsInvalid()
    {
        var productId = 1;
        var updateCommand = new UpdateProductCommand
        {
            Name = "",
            Description = "Updated Description",
            Price = -10, 
            IsAvailable = true,
            User = Controller.User
        };

        MediatorMock.Setup(m => m.Send(It.Is<UpdateProductCommand>(c =>
            c.ProductId == productId && c.User == Controller.User), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid product data."));
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            Controller.UpdateProduct(productId, updateCommand));

        Assert.Equal("Invalid product data.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<UpdateProductCommand>(c =>
            c.ProductId == productId && c.User == Controller.User), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNoContent_WhenNoChangesAreMade()
    {
        var productId = 1;
        var updateCommand = new UpdateProductCommand
        {
            Name = "Original Product",
            Description = "Original Description",
            Price = 100.00m,
            IsAvailable = true,
            User = Controller.User
        };

        MediatorMock.Setup(m => m.Send(It.Is<UpdateProductCommand>(c =>
            c.ProductId == productId && c.User == Controller.User), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));
        
        var result = await Controller.UpdateProduct(productId, updateCommand);
        
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<UpdateProductCommand>(c =>
            c.ProductId == productId && c.User == Controller.User), It.IsAny<CancellationToken>()), Times.Once);
    }
}