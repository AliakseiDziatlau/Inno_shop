using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserControl.Application.Commands.AuthsControllerCommands;

namespace UserControlTests.UserControl.UnitTests.AuthsControllerTests;

public class RequestPasswordResetTests : AuthsControllerTestsBase
{
    [Fact]
    public async Task RequestPasswordReset_ReturnsOkResult_WhenEmailIsValid()
    {
        var command = new RequestPasswordResetCommand
        {
            Email = "testuser@example.com"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.RequestPasswordReset(command); 

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Password reset link sent to your email.", okResult.Value);

        MediatorMock.Verify(m => m.Send(It.Is<RequestPasswordResetCommand>(c => c.Email == command.Email), It.IsAny<CancellationToken>()), Times.Once);
    }

   [Fact]
    public async Task RequestPasswordReset_ThrowsKeyNotFoundException_WhenEmailDoesNotExist()
    {
        var command = new RequestPasswordResetCommand
        {
            Email = "nonexistent@example.com"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("User with the specified email was not found."));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.RequestPasswordReset(command));

        Assert.Equal("User with the specified email was not found.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<RequestPasswordResetCommand>(c => c.Email == command.Email), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestPasswordReset_ThrowsInvalidOperationException_WhenEmailServiceFails()
    {
        var command = new RequestPasswordResetCommand
        {
            Email = "testuser@example.com"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Failed to send email."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.RequestPasswordReset(command));

        Assert.Equal("Failed to send email.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<RequestPasswordResetCommand>(c => c.Email == command.Email), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestPasswordReset_ThrowsArgumentException_WhenEmailIsEmpty()
    {
        var command = new RequestPasswordResetCommand
        {
            Email = ""
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Email cannot be empty."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.RequestPasswordReset(command));

        Assert.Equal("Email cannot be empty.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<RequestPasswordResetCommand>(c => c.Email == command.Email), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestPasswordReset_ThrowsArgumentException_WhenEmailFormatIsInvalid()
    {
        var command = new RequestPasswordResetCommand
        {
            Email = "invalid-email-format"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid email format."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.RequestPasswordReset(command));

        Assert.Equal("Invalid email format.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<RequestPasswordResetCommand>(c => c.Email == command.Email), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestPasswordReset_ThrowsException_WhenUnexpectedErrorOccurs()
    {
        var command = new RequestPasswordResetCommand
        {
            Email = "testuser@example.com"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error occurred."));

        var exception = await Assert.ThrowsAsync<Exception>(() => Controller.RequestPasswordReset(command));

        Assert.Equal("Unexpected error occurred.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<RequestPasswordResetCommand>(c => c.Email == command.Email), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestPasswordReset_LogsMessage_WhenEmailIsSentSuccessfully()
    {
        var command = new RequestPasswordResetCommand
        {
            Email = "testuser@example.com"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.RequestPasswordReset(command);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Password reset link sent to your email.", okResult.Value);

        MediatorMock.Verify(m => m.Send(It.Is<RequestPasswordResetCommand>(c => c.Email == command.Email), It.IsAny<CancellationToken>()), Times.Once);
    }
}