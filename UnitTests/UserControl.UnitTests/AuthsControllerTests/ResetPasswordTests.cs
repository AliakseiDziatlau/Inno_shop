using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserControl.Application.Commands.AuthsControllerCommands;

namespace UserControlTests.UserControl.UnitTests.AuthsControllerTests;

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
}