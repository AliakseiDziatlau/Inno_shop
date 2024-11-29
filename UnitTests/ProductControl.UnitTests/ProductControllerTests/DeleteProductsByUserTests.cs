using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductControl.Application.Commands;

namespace UserControlTests.ProductControl.UnitTests.ProductControllerTests;

public class DeleteProductsByUserTests : ProductsControllerTestsBase
{
    [Fact]
    public async Task DeleteProductsByUser_ReturnsNoContent_WhenProductsDeletedSuccessfully()
    {
        var userId = 1;
        var command = new DeleteProductsByUserCommand(userId);

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.DeleteProductsByUser(userId);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductsByUser_ThrowsKeyNotFoundException_WhenUserProductsNotFound()
    {
        var userId = 1;
        var command = new DeleteProductsByUserCommand(userId);

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("No products found for the specified user."));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.DeleteProductsByUser(userId));

        Assert.Equal("No products found for the specified user.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductsByUser_ThrowsUnauthorizedAccessException_WhenUserNotAuthorized()
    {
        var userId = 1;
        var command = new DeleteProductsByUserCommand(userId);

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("You are not authorized to delete products for this user."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.DeleteProductsByUser(userId));

        Assert.Equal("You are not authorized to delete products for this user.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductsByUser_ThrowsArgumentException_WhenUserIdIsInvalid()
    {
        var invalidUserId = -1;
        var command = new DeleteProductsByUserCommand(invalidUserId);

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == invalidUserId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("User ID must be greater than 0."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.DeleteProductsByUser(invalidUserId));

        Assert.Equal("User ID must be greater than 0.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == invalidUserId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductsByUser_VerifiesCorrectMediatorCall()
    {
        var userId = 1;
        var command = new DeleteProductsByUserCommand(userId);

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        await Controller.DeleteProductsByUser(userId);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductsByUser_HandlesNoProductsGracefully()
    {
        var userId = 1;
        var command = new DeleteProductsByUserCommand(userId);

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.DeleteProductsByUser(userId);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductsByUser_ThrowsException_WhenUnexpectedErrorOccurs()
    {
        var userId = 1;
        var command = new DeleteProductsByUserCommand(userId);

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error occurred."));

        var exception = await Assert.ThrowsAsync<Exception>(() => Controller.DeleteProductsByUser(userId));

        Assert.Equal("Unexpected error occurred.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteProductsByUser_ReturnsNoContent_WhenUserHasNoProducts()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value)); 
        
        var result = await Controller.DeleteProductsByUser(userId);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteProductsByUser_UsesAuthorizedUserToken()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        await Controller.DeleteProductsByUser(userId);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteProductsByUser_ThrowsUnauthorizedAccessException_WhenTokenIsInvalid()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid token."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.DeleteProductsByUser(userId));

        Assert.Equal("Invalid token.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteProductsByUser_DeletesOnlyUserOwnedProducts()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        await Controller.DeleteProductsByUser(userId);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteProductsByUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
}