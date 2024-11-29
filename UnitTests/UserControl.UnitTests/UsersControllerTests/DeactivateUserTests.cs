using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using UserControl.Application.Commands.UsersControllerCommands;

namespace UserControlTests.UserControl.UnitTests.UsersControllerTests;

public class DeactivateUserTests : UsersControllerTestsBase
{
    [Fact]
    public async Task DeactivateUser_ReturnsNoContent_WhenUserIsDeactivatedSuccessfully()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.DeactivateUser(userId);

        Assert.IsType<NoContentResult>(result);
        MediatorMock.Verify(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateUser_ThrowsKeyNotFoundException_WhenUserDoesNotExist()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException($"User with ID {userId} not found."));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.DeactivateUser(userId));

        Assert.Equal($"User with ID {userId} not found.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateUser_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthorized()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Not authorized to deactivate the user."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.DeactivateUser(userId));

        Assert.Equal("Not authorized to deactivate the user.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateUser_ThrowsDbUpdateException_WhenDatabaseErrorOccurs()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Database error occurred."));

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => Controller.DeactivateUser(userId));

        Assert.Equal("Database error occurred.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateUser_ThrowsArgumentException_WhenUserIdIsInvalid()
    {
        var invalidUserId = -1;

        MediatorMock.Setup(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == invalidUserId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("User ID must be greater than zero."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.DeactivateUser(invalidUserId));

        Assert.Equal("User ID must be greater than zero.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == invalidUserId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateUser_ThrowsConcurrencyException_WhenConcurrentUpdateOccurs()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateConcurrencyException("Concurrent update error."));

        var exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => Controller.DeactivateUser(userId));

        Assert.Equal("Concurrent update error.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateUser_CompletesWithinExpectedTime()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await Controller.DeactivateUser(userId);
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Method took too long to execute.");
        MediatorMock.Verify(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateUser_ThrowsInvalidOperationException_WhenUserIsAlreadyDeactivated()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("User is already deactivated."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.DeactivateUser(userId));

        Assert.Equal("User is already deactivated.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.Is<DeactivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
}