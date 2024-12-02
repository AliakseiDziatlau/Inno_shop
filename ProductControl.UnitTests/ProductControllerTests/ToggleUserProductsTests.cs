using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductControl.Application.Commands;

namespace UserControlTests.ProductControllerTests;

public class ToggleUserProductsTests : ProductsControllerTestsBase
{
    [Fact]
    public async Task ToggleUserProducts_ReturnsNoContent_WhenProductsAreToggledSuccessfully()
    {
        var userId = 1;
        var isActive = true;

        MediatorMock.Setup(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);
        
        var result = await Controller.ToggleUserProducts(userId, isActive);
        
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleUserProducts_ThrowsKeyNotFoundException_WhenUserProductsNotFound()
    {
        var userId = 1;
        var isActive = false;

        MediatorMock.Setup(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("No products found for the specified user."));
        
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.ToggleUserProducts(userId, isActive));
        Assert.Equal("No products found for the specified user.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleUserProducts_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthorized()
    {
        var userId = 1;
        var isActive = true;

        MediatorMock.Setup(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("You are not authorized to toggle these products."));
        
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.ToggleUserProducts(userId, isActive));
        Assert.Equal("You are not authorized to toggle these products.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleUserProducts_ThrowsArgumentException_WhenUserIdIsInvalid()
    {
        var invalidUserId = 0;
        var isActive = true;

        MediatorMock.Setup(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == invalidUserId && c.IsActive == isActive), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid user ID."));
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.ToggleUserProducts(invalidUserId, isActive));
        Assert.Equal("Invalid user ID.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == invalidUserId && c.IsActive == isActive), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleUserProducts_ThrowsException_WhenServiceFails()
    {
        var userId = 1;
        var isActive = true;

        MediatorMock.Setup(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("An unexpected error occurred."));
        
        var exception = await Assert.ThrowsAsync<Exception>(() => Controller.ToggleUserProducts(userId, isActive));
        Assert.Equal("An unexpected error occurred.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleUserProducts_ReturnsNoContent_WhenUserHasManyProducts()
    {
        var userId = 1;
        var isActive = true;

        MediatorMock.Setup(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);
        
        var result = await Controller.ToggleUserProducts(userId, isActive);
        
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleUserProducts_DoesNotCauseUnnecessaryChanges_WhenCalledMultipleTimes()
    {
        var userId = 1;
        var isActive = true;

        MediatorMock.Setup(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);
        
        await Controller.ToggleUserProducts(userId, isActive);
        await Controller.ToggleUserProducts(userId, isActive);
        
        MediatorMock.Verify(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
    
    [Fact]
    public async Task ToggleUserProducts_ReturnsNoContent_WhenUserHasNoProducts()
    {
        var userId = 1;
        var isActive = false;

        MediatorMock.Setup(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await Controller.ToggleUserProducts(userId, isActive);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ToggleUserProducts_ThrowsPartialFailureException_WhenSomeProductsCannotBeUpdated()
    {
        var userId = 1;
        var isActive = true;

        MediatorMock.Setup(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Some products could not be updated."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.ToggleUserProducts(userId, isActive));
        Assert.Equal("Some products could not be updated.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ToggleUserProducts_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        var userId = 1;
        var isActive = true;

        Controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }; 

        MediatorMock.Setup(m => m.Send(It.IsAny<ToggleUserProductsCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("User is not authenticated."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.ToggleUserProducts(userId, isActive));

        Assert.Equal("User is not authenticated.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.IsAny<ToggleUserProductsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ToggleUserProducts_DoesNotChangeState_WhenProductsAlreadyMatchIsActive()
    {
        var userId = 1;
        var isActive = true;

        MediatorMock.Setup(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        await Controller.ToggleUserProducts(userId, isActive);

        MediatorMock.Verify(m => m.Send(It.Is<ToggleUserProductsCommand>(c => c.UserId == userId && c.IsActive == isActive), It.IsAny<CancellationToken>()), Times.Once);
    }
}
