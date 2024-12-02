using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserControl.Application.Commands.AuthsControllerCommands;

namespace UserControl.UnitTests.AuthsControllerTests;

public class ConfirmEmailTests : AuthsControllerTestsBase
{
    [Fact]
    public async Task ConfirmEmail_ReturnsOkResult_WhenTokenIsValid()
    {
        var token = "valid-confirmation-token";
    
        MediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));
    
        var result = await Controller.ConfirmEmail(token);
    
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Your account has been confirmed successfully.", okResult.Value);
    
        MediatorMock.Verify(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == token), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_ThrowsKeyNotFoundException_WhenTokenIsInvalid()
    {
        var token = "invalid-confirmation-token";

        MediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Invalid confirmation token."));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.ConfirmEmail(token));

        Assert.Equal("Invalid confirmation token.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == token), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_ThrowsUnauthorizedAccessException_WhenTokenIsExpired()
    {
        var token = "expired-confirmation-token";

        MediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Confirmation token has expired."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.ConfirmEmail(token));

        Assert.Equal("Confirmation token has expired.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == token), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_ThrowsException_WhenServiceFails()
    {
        var token = "any-confirmation-token";

        MediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error occurred."));

        var exception = await Assert.ThrowsAsync<Exception>(() => Controller.ConfirmEmail(token));

        Assert.Equal("Unexpected error occurred.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == token), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_ThrowsArgumentException_WhenTokenIsEmpty()
    {
        var emptyToken = "";

        MediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Confirmation token cannot be empty."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.ConfirmEmail(emptyToken));

        Assert.Equal("Confirmation token cannot be empty.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == emptyToken), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_ThrowsArgumentNullException_WhenTokenIsNull()
    {
        string nullToken = null;

        MediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentNullException(nameof(nullToken), "Confirmation token cannot be null."));

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => Controller.ConfirmEmail(nullToken));

        Assert.Equal("Confirmation token cannot be null. (Parameter 'nullToken')", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == nullToken), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_ThrowsInvalidOperationException_WhenTokenIsReused()
    {
        var token = "already-used-token";

        MediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Token has already been used."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Controller.ConfirmEmail(token));

        Assert.Equal("Token has already been used.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == token), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ConfirmEmail_ThrowsArgumentException_WhenTokenIsWhitespace()
    {
        var whitespaceToken = "   ";

        MediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Confirmation token cannot be whitespace."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.ConfirmEmail(whitespaceToken));

        Assert.Equal("Confirmation token cannot be whitespace.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == whitespaceToken), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ConfirmEmail_ReturnsOkResult_WhenTokenIsValidWithDifferentCase()
    {
        var validToken = "Valid-Token";
        var tokenWithDifferentCase = "valid-token";

        MediatorMock
            .Setup(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == tokenWithDifferentCase), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.ConfirmEmail(tokenWithDifferentCase);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Your account has been confirmed successfully.", okResult.Value);

        MediatorMock.Verify(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == tokenWithDifferentCase), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ConfirmEmail_ThrowsArgumentException_WhenTokenExceedsMaximumLength()
    {
        var longToken = new string('a', 1001); 

        MediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Confirmation token exceeds maximum length."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.ConfirmEmail(longToken));

        Assert.Equal("Confirmation token exceeds maximum length.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == longToken), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ConfirmEmail_ReturnsOkResult_WhenTokenContainsSpecialCharacters()
    {
        var token = "valid-token-@#$%";

        MediatorMock
            .Setup(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == token), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.ConfirmEmail(token);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Your account has been confirmed successfully.", okResult.Value);

        MediatorMock.Verify(m => m.Send(It.Is<ConfirmEmailCommand>(cmd => cmd.Token == token), It.IsAny<CancellationToken>()), Times.Once);
    }
}