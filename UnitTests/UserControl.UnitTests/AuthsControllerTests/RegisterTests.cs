using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserControl.Application.Commands.AuthsControllerCommands;

namespace UserControlTests.UserControl.UnitTests.AuthsControllerTests;

public class RegisterTests : AuthsControllerTestsBase
{
    [Fact]
    public async Task Register_ReturnsOkResult_WhenRegistrationIsSuccessful()
    {
        var command = new RegisterCommand
        {
            Name = "John Doe",
            Email = "johndoe@example.com",
            Password = "SecurePassword123",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.Register(command);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Registration successful! Please check your email to confirm your account.", okResult.Value);
        MediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_ThrowsArgumentException_WhenEmailAlreadyExists()
    {
        var command = new RegisterCommand
        {
            Name = "John Doe",
            Email = "existinguser@example.com",
            Password = "SecurePassword123",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new ArgumentException("A user with this email already exists."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            Controller.Register(command));

        Assert.Equal("A user with this email already exists.", exception.Message);
        MediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_ThrowsInvalidOperationException_WhenEmailServiceFails()
    {
        var command = new RegisterCommand
        {
            Name = "Jane Doe",
            Email = "janedoe@example.com",
            Password = "SecurePassword123",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new InvalidOperationException("Failed to send confirmation email."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Controller.Register(command));

        Assert.Equal("Failed to send confirmation email.", exception.Message);
        MediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_ThrowsArgumentException_WhenNameIsEmpty()
    {
        var command = new RegisterCommand
        {
            Name = "",
            Email = "johndoe@example.com",
            Password = "SecurePassword123",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new ArgumentException("Name cannot be empty."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            Controller.Register(command));

        Assert.Equal("Name cannot be empty.", exception.Message);
        MediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_ThrowsArgumentException_WhenEmailIsEmpty()
    {
        var command = new RegisterCommand
        {
            Name = "John Doe",
            Email = "",
            Password = "SecurePassword123",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new ArgumentException("Email cannot be empty."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            Controller.Register(command));

        Assert.Equal("Email cannot be empty.", exception.Message);
        MediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_ThrowsArgumentException_WhenEmailIsInvalid()
    {
        var command = new RegisterCommand
        {
            Name = "John Doe",
            Email = "invalid-email",
            Password = "SecurePassword123",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new ArgumentException("Invalid email format."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            Controller.Register(command));

        Assert.Equal("Invalid email format.", exception.Message);
        MediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_ThrowsArgumentException_WhenPasswordIsEmpty()
    {
        var command = new RegisterCommand
        {
            Name = "John Doe",
            Email = "johndoe@example.com",
            Password = "",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new ArgumentException("Password cannot be empty."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            Controller.Register(command));

        Assert.Equal("Password cannot be empty.", exception.Message);
        MediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_ThrowsArgumentException_WhenRoleIsInvalid()
    {
        var command = new RegisterCommand
        {
            Name = "John Doe",
            Email = "johndoe@example.com",
            Password = "SecurePassword123",
            Role = "InvalidRole"
        };

        MediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new ArgumentException("Invalid role specified."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            Controller.Register(command));

        Assert.Equal("Invalid role specified.", exception.Message);
        MediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }
}