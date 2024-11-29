using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using UserControl.Application.Commands.UsersControllerCommands;

namespace UserControlTests.UserControl.UnitTests.UsersControllerTests;

public class ActivateUserTests : UsersControllerTestsBase
{
    [Fact]
    public async Task ActivateUser_ReturnsNoContent_WhenUserIsActivatedSuccessfully()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.ActivateUser(userId);

        Assert.IsType<NoContentResult>(result);
        MediatorMock.Verify(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActivateUser_ThrowsKeyNotFoundException_WhenUserDoesNotExist()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException($"User with ID {userId} not found."));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.ActivateUser(userId));

        Assert.Equal($"User with ID {userId} not found.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActivateUser_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthorized()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Not authorized to activate the user."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.ActivateUser(userId));

        Assert.Equal("Not authorized to activate the user.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActivateUser_ThrowsDbUpdateException_WhenDatabaseErrorOccurs()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Database error occurred."));

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => Controller.ActivateUser(userId));

        Assert.Equal("Database error occurred.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActivateUser_ThrowsArgumentException_WhenUserIdIsInvalid()
    {
        var invalidUserId = -1;

        MediatorMock.Setup(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == invalidUserId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("User ID must be greater than zero."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.ActivateUser(invalidUserId));

        Assert.Equal("User ID must be greater than zero.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == invalidUserId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActivateUser_ThrowsConcurrencyException_WhenConcurrentUpdateOccurs()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateConcurrencyException("Concurrent update error."));

        var exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => Controller.ActivateUser(userId));

        Assert.Equal("Concurrent update error.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActivateUser_CompletesWithinExpectedTime()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await Controller.ActivateUser(userId);
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Method took too long to execute.");
        MediatorMock.Verify(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ActivateUser_DoesNotChangeState_WhenUserIsAlreadyActive()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("User is already active."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.ActivateUser(userId));

        Assert.Equal("User is already active.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.Is<ActivateUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
}