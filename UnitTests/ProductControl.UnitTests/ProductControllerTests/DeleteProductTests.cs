using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductControl.Application.Commands;

namespace UserControlTests.ProductControl.UnitTests.ProductControllerTests;

public class DeleteProductTests : ProductsControllerTestsBase
{
    [Fact]
    public async Task DeleteProduct_ReturnsNoContent_WhenProductIsDeletedSuccessfully()
    {
        var productId = 1;
        var user = new ClaimsPrincipal();

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.DeleteProduct(productId);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductCommand>(c => c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()), Times.Never);
    }
    

    [Fact]
    public async Task DeleteProduct_VerifiesCorrectMediatorCall()
    {
        var productId = 1;
        var user = new ClaimsPrincipal();

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        await Controller.DeleteProduct(productId);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductCommand>(c => c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNoContent_WhenProductAlreadyDeleted()
    {
        var productId = 1;
        var user = new ClaimsPrincipal();

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.DeleteProduct(productId);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductCommand>(c => c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    
    
    [Fact]
    public async Task DeleteProduct_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthorized()
    {
        var productId = 1;
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "userId")
        }));
    
        Controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };
    
        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => 
                c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("You are not authorized to delete this product."));
    
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.DeleteProduct(productId));
    
        Assert.Equal("You are not authorized to delete this product.", exception.Message);
    
        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductCommand>(c => 
            c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var productId = 999;
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "userId")
        }));

        Controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => 
                c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Product not found."));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.DeleteProduct(productId));

        Assert.Equal("Product not found.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductCommand>(c => 
            c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteProduct_ThrowsInvalidOperationException_WhenProductAlreadyDeleted()
    {
        var productId = 1;
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "userId")
        }));

        Controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => 
                c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Product has already been deleted."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.DeleteProduct(productId));

        Assert.Equal("Product has already been deleted.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductCommand>(c => 
            c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteProduct_ThrowsArgumentException_WhenProductIdIsInvalid()
    {
        var invalidProductId = 0;
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "userId")
        }));

        Controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => 
                c.ProductId == invalidProductId && c.User == user), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Product ID must be greater than zero."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.DeleteProduct(invalidProductId));

        Assert.Equal("Product ID must be greater than zero.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductCommand>(c => 
            c.ProductId == invalidProductId && c.User == user), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteProduct_ThrowsException_WhenUnexpectedErrorOccurs()
    {
        var productId = 1;
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "userId")
        }));

        Controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => 
                c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error occurred."));

        var exception = await Assert.ThrowsAsync<Exception>(() => Controller.DeleteProduct(productId));

        Assert.Equal("Unexpected error occurred.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductCommand>(c => 
            c.ProductId == productId && c.User == user), It.IsAny<CancellationToken>()), Times.Once);
    }
}
