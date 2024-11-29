using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductControl.Application.Commands;
using ProductControl.Application.DTOs;

namespace UserControlTests.ProductControl.UnitTests.ProductControllerTests;

public class CreateProductTests : ProductsControllerTestsBase
{
    [Fact]
    public async Task CreateProduct_ReturnsCreatedAtAction_WhenProductIsCreatedSuccessfully()
    {
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            IsAvailable = true,
            User = new ClaimsPrincipal() 
        };

        var createdProduct = new ProductDto
        {
            Id = 100,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            IsAvailable = command.IsAvailable
        };

        MediatorMock.Setup(m => m.Send(It.Is<CreateProductCommand>(c => c.Name == command.Name), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProduct);

        var result = await Controller.CreateProduct(command);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(Controller.GetProductById), createdAtActionResult.ActionName);

        var returnedProduct = Assert.IsType<ProductDto>(createdAtActionResult.Value);
        Assert.Equal(createdProduct.Id, returnedProduct.Id);
        Assert.Equal(createdProduct.Name, returnedProduct.Name);
        Assert.Equal(createdProduct.Description, returnedProduct.Description);
        Assert.Equal(createdProduct.Price, returnedProduct.Price);
        Assert.Equal(createdProduct.IsAvailable, returnedProduct.IsAvailable);

        MediatorMock.Verify(m => m.Send(It.Is<CreateProductCommand>(c => c.Name == command.Name), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_ThrowsArgumentException_WhenInvalidDataIsProvided()
    {
        var command = new CreateProductCommand
        {
            Name = "",
            Description = "Test Description",
            Price = 99.99m,
            IsAvailable = true,
            User = new ClaimsPrincipal()
        };

        MediatorMock.Setup(m => m.Send(It.Is<CreateProductCommand>(c => c.Name == command.Name), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid product data"));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.CreateProduct(command));
        Assert.Equal("Invalid product data", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<CreateProductCommand>(c => c.Name == command.Name), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthorized()
    {
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            IsAvailable = true,
            User = null 
        };

        MediatorMock.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("User is not authorized"));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.CreateProduct(command));
        Assert.Equal("User is not authorized", exception.Message);

        MediatorMock.Verify(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_ThrowsKeyNotFoundException_WhenUserDoesNotExist()
    {
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            IsAvailable = true,
            User = new ClaimsPrincipal()
        };

        MediatorMock.Setup(m => m.Send(It.Is<CreateProductCommand>(c => c.Name == command.Name), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("User not found"));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.CreateProduct(command));
        Assert.Equal("User not found", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<CreateProductCommand>(c => c.Name == command.Name), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_ThrowsValidationException_WhenValidationFails()
    {
        var command = new CreateProductCommand
        {
            Name = "Invalid Product",
            Description = "Test Description",
            Price = -10.0m, 
            IsAvailable = true,
            User = new ClaimsPrincipal()
        };

        MediatorMock.Setup(m => m.Send(It.Is<CreateProductCommand>(c => c.Price == command.Price), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("Price must be greater than zero."));

        var exception = await Assert.ThrowsAsync<ValidationException>(() => Controller.CreateProduct(command));
        Assert.Equal("Price must be greater than zero.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<CreateProductCommand>(c => c.Price == command.Price), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_VerifiesCorrectMediatorCalls()
    {
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            IsAvailable = true,
            User = new ClaimsPrincipal()
        };

        MediatorMock.Setup(m => m.Send(It.Is<CreateProductCommand>(c => c.Name == command.Name), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductDto
            {
                Id = 100,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                IsAvailable = command.IsAvailable
            });

        await Controller.CreateProduct(command);

        MediatorMock.Verify(m => m.Send(It.Is<CreateProductCommand>(c => c.Name == command.Name), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateProduct_ReturnsCreatedAtAction_WhenProductHasMaximalValues()
    {
        var command = new CreateProductCommand
        {
            Name = new string('A', 255), 
            Description = new string('B', 1000), 
            Price = decimal.MaxValue,
            IsAvailable = true,
            User = new ClaimsPrincipal()
        };

        var createdProduct = new ProductDto
        {
            Id = 101,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            IsAvailable = command.IsAvailable
        };

        MediatorMock.Setup(m => m.Send(It.Is<CreateProductCommand>(c => c.Name == command.Name), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProduct);

        var result = await Controller.CreateProduct(command);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedProduct = Assert.IsType<ProductDto>(createdAtActionResult.Value);

        Assert.Equal(createdProduct.Id, returnedProduct.Id);
        Assert.Equal(createdProduct.Name, returnedProduct.Name);
        Assert.Equal(createdProduct.Description, returnedProduct.Description);
        Assert.Equal(createdProduct.Price, returnedProduct.Price);
        Assert.Equal(createdProduct.IsAvailable, returnedProduct.IsAvailable);

        MediatorMock.Verify(m => m.Send(It.Is<CreateProductCommand>(c => c.Name == command.Name), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateProduct_ThrowsValidationException_WhenDescriptionIsEmpty()
    {
        var command = new CreateProductCommand
        {
            Name = "Valid Product",
            Description = "",
            Price = 99.99m,
            IsAvailable = true,
            User = new ClaimsPrincipal()
        };

        MediatorMock.Setup(m => m.Send(It.Is<CreateProductCommand>(c => c.Description == command.Description), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("Product description is required."));

        var exception = await Assert.ThrowsAsync<ValidationException>(() => Controller.CreateProduct(command));
        Assert.Equal("Product description is required.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<CreateProductCommand>(c => c.Description == command.Description), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateProduct_ReturnsCreatedAtAction_WhenProductHasMinimalValues()
    {
        var command = new CreateProductCommand
        {
            Name = "A", 
            Description = "B", 
            Price = 0.01m, 
            IsAvailable = false,
            User = new ClaimsPrincipal()
        };

        var createdProduct = new ProductDto
        {
            Id = 102,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            IsAvailable = command.IsAvailable
        };

        MediatorMock.Setup(m => m.Send(It.Is<CreateProductCommand>(c => c.Name == command.Name), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProduct);

        var result = await Controller.CreateProduct(command);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedProduct = Assert.IsType<ProductDto>(createdAtActionResult.Value);

        Assert.Equal(createdProduct.Id, returnedProduct.Id);
        Assert.Equal(createdProduct.Name, returnedProduct.Name);
        Assert.Equal(createdProduct.Description, returnedProduct.Description);
        Assert.Equal(createdProduct.Price, returnedProduct.Price);
        Assert.Equal(createdProduct.IsAvailable, returnedProduct.IsAvailable);

        MediatorMock.Verify(m => m.Send(It.Is<CreateProductCommand>(c => c.Name == command.Name), It.IsAny<CancellationToken>()), Times.Once);
    }
}