using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserControl.Application.Commands.AuthsControllerCommands;

namespace UserControl.UnitTests.AuthsControllerTests;

public class ResetPasswordTests : AuthsControllerTestsBase
{
    [Fact]
    public async Task ResetPassword_ReturnsOkResult_WhenResetIsSuccessful()
    {
        var token = "valid-token";
        var command = new ResetPasswordCommand
        {
            NewPassword = "NewSecurePassword123"
        };

        MediatorMock.Setup(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await Controller.ResetPassword(token, command);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Password has been reset successfully.", okResult.Value);

        MediatorMock.Verify(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_ThrowsUnauthorizedAccessException_WhenTokenIsInvalid()
    {
        var token = "invalid-token";
        var command = new ResetPasswordCommand
        {
            NewPassword = "NewSecurePassword123"
        };

        MediatorMock.Setup(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid or expired password reset token."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.ResetPassword(token, command));

        Assert.Equal("Invalid or expired password reset token.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_ThrowsInvalidOperationException_WhenPasswordUpdateFails()
    {
        var token = "valid-token";
        var command = new ResetPasswordCommand
        {
            NewPassword = "NewSecurePassword123"
        };

        MediatorMock.Setup(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Failed to update password."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.ResetPassword(token, command));

        Assert.Equal("Failed to update password.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_ThrowsArgumentException_WhenTokenIsEmpty()
    {
        var token = "";
        var command = new ResetPasswordCommand
        {
            NewPassword = "NewSecurePassword123"
        };

        MediatorMock.Setup(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Reset token cannot be empty."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.ResetPassword(token, command));

        Assert.Equal("Reset token cannot be empty.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_ThrowsArgumentException_WhenPasswordIsEmpty()
    {
        var token = "valid-token";
        var command = new ResetPasswordCommand
        {
            NewPassword = ""
        };

        MediatorMock.Setup(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Password cannot be empty."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.ResetPassword(token, command));

        Assert.Equal("Password cannot be empty.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_ThrowsException_WhenUnexpectedErrorOccurs()
    {
        var token = "valid-token";
        var command = new ResetPasswordCommand
        {
            NewPassword = "NewSecurePassword123"
        };

        MediatorMock.Setup(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error occurred."));

        var exception = await Assert.ThrowsAsync<Exception>(() => Controller.ResetPassword(token, command));

        Assert.Equal("Unexpected error occurred.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ResetPassword_ThrowsArgumentException_WhenPasswordIsTooShort()
    {
        var token = "valid-token";
        var command = new ResetPasswordCommand
        {
            NewPassword = "123"
        };

        MediatorMock.Setup(m => m.Send(It.Is<ResetPasswordCommand>(c =>
                c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Password must be at least 8 characters long."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.ResetPassword(token, command));

        Assert.Equal("Password must be at least 8 characters long.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ResetPassword_ThrowsArgumentException_WhenPasswordLacksSpecialCharacters()
    {
        var token = "valid-token";
        var command = new ResetPasswordCommand
        {
            NewPassword = "SimplePassword123"
        };

        MediatorMock.Setup(m => m.Send(It.Is<ResetPasswordCommand>(c =>
                c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Password must contain at least one special character."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.ResetPassword(token, command));

        Assert.Equal("Password must contain at least one special character.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ResetPassword_ThrowsInvalidOperationException_WhenTokenIsReused()
    {
        var token = "used-token";
        var command = new ResetPasswordCommand
        {
            NewPassword = "NewSecurePassword123"
        };

        MediatorMock.Setup(m => m.Send(It.Is<ResetPasswordCommand>(c =>
                c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("The reset token has already been used."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.ResetPassword(token, command));

        Assert.Equal("The reset token has already been used.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ResetPassword_ThrowsUnauthorizedAccessException_WhenTokenIsExpired()
    {
        var token = "expired-token";
        var command = new ResetPasswordCommand
        {
            NewPassword = "NewSecurePassword123"
        };

        MediatorMock.Setup(m => m.Send(It.Is<ResetPasswordCommand>(c =>
                c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("The reset token has expired."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.ResetPassword(token, command));

        Assert.Equal("The reset token has expired.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ResetPassword_ThrowsArgumentNullException_WhenTokenIsNull()
    {
        string token = null;
        var command = new ResetPasswordCommand
        {
            NewPassword = "NewSecurePassword123"
        };

        MediatorMock.Setup(m => m.Send(It.Is<ResetPasswordCommand>(c =>
                c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException(nameof(token), "Reset token cannot be null."));

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => Controller.ResetPassword(token, command));

        Assert.Equal("Reset token cannot be null. (Parameter 'token')", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ResetPassword_ReturnsOkResult_WhenPasswordIsMaximal()
    {
        var token = "valid-token";
        var command = new ResetPasswordCommand
        {
            NewPassword = new string('P', 128) 
        };

        MediatorMock.Setup(m => m.Send(It.Is<ResetPasswordCommand>(c =>
                c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await Controller.ResetPassword(token, command);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Password has been reset successfully.", okResult.Value);

        MediatorMock.Verify(m => m.Send(It.Is<ResetPasswordCommand>(c =>
            c.Token == token && c.NewPassword == command.NewPassword), It.IsAny<CancellationToken>()), Times.Once);
    }
}