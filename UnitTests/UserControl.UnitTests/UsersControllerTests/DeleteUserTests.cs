using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using UserControl.Application.Commands.UsersControllerCommands;

namespace UserControlTests.UserControl.UnitTests.UsersControllerTests;

public class DeleteUserTests : UsersControllerTestsBase
{
    [Fact]
    public async Task DeleteUser_ReturnsNoContent_WhenUserIsDeletedSuccessfully()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.DeleteUser(userId);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ThrowsKeyNotFoundException_WhenUserDoesNotExist()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException($"User with ID {userId} not found."));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.DeleteUser(userId));

        Assert.Equal($"User with ID {userId} not found.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthorized()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Not authorized to delete this user."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.DeleteUser(userId));

        Assert.Equal("Not authorized to delete this user.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ThrowsInvalidOperationException_WhenUserIsAlreadyDeleted()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("User is already deleted."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.DeleteUser(userId));

        Assert.Equal("User is already deleted.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ThrowsDbUpdateException_WhenDatabaseErrorOccurs()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Database error occurred."));

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => Controller.DeleteUser(userId));

        Assert.Equal("Database error occurred.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ThrowsArgumentException_WhenUserIdIsInvalid()
    {
        var invalidUserId = -1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == invalidUserId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("User ID must be greater than zero."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.DeleteUser(invalidUserId));

        Assert.Equal("User ID must be greater than zero.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == invalidUserId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_CompletesWithinExpectedTime()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await Controller.DeleteUser(userId);
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Method took too long to execute.");
        MediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteUser_ThrowsInvalidOperationException_WhenUserTriesToDeleteSelf()
    {
        var selfUserId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == selfUserId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("You cannot delete your own account."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.DeleteUser(selfUserId));

        Assert.Equal("You cannot delete your own account.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == selfUserId), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteUser_ThrowsUnauthorizedAccessException_WhenUserIsNotAdmin()
    {
        var userId = 2;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Only admins can delete users."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.DeleteUser(userId));

        Assert.Equal("Only admins can delete users.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteUser_DoesNotChangeState_WhenDatabaseErrorOccurs()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Database error occurred."));

        await Assert.ThrowsAsync<DbUpdateException>(() => Controller.DeleteUser(userId));

        MediatorMock.Verify(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
}