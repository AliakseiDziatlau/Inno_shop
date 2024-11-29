using Microsoft.AspNetCore.Mvc;
using Moq;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.DTOs;

namespace UserControlTests.UserControl.UnitTests.UsersControllerTests;

public class GetUserByIdTests : UsersControllerTestsBase
{
    [Fact]
    public async Task GetUserById_ReturnsOkResult_WhenUserExists()
    {
        var userId = 1;
        var userDto = new UserDto
        {
            Id = userId,
            Name = "Test User",
            Email = "testuser@example.com",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(It.Is<GetUserByIdCommand>(q => q.UserId == userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userDto);

        var result = await Controller.GetUserById(userId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);

        Assert.Equal(userDto.Id, returnedUser.Id);
        Assert.Equal(userDto.Name, returnedUser.Name);
        Assert.Equal(userDto.Email, returnedUser.Email);
        Assert.Equal(userDto.Role, returnedUser.Role);

        MediatorMock.Verify(m => m.Send(It.Is<GetUserByIdCommand>(q => q.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthorized()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<GetUserByIdCommand>(q => q.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("You are not authorized to access this user."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.GetUserById(userId));

        Assert.Equal("You are not authorized to access this user.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<GetUserByIdCommand>(q => q.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ThrowsException_WhenUnexpectedErrorOccurs()
    {
        var userId = 1;

        MediatorMock.Setup(m => m.Send(It.Is<GetUserByIdCommand>(q => q.UserId == userId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error occurred."));

        var exception = await Assert.ThrowsAsync<Exception>(() => Controller.GetUserById(userId));

        Assert.Equal("Unexpected error occurred.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<GetUserByIdCommand>(q => q.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ThrowsArgumentException_WhenUserIdIsInvalid()
    {
        var invalidUserId = 0;

        MediatorMock.Setup(m => m.Send(It.Is<GetUserByIdCommand>(q => q.UserId == invalidUserId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("User ID must be greater than 0."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.GetUserById(invalidUserId));

        Assert.Equal("User ID must be greater than 0.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<GetUserByIdCommand>(q => q.UserId == invalidUserId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserById_WaitsForServiceToComplete_WhenServiceIsDelayed()
    {
        var userId = 1;
        var userDto = new UserDto
        {
            Id = userId,
            Name = "Test User",
            Email = "testuser@example.com",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(It.Is<GetUserByIdCommand>(q => q.UserId == userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                Thread.Sleep(100);
                return userDto;
            });

        var result = await Controller.GetUserById(userId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(userDto.Id, returnedUser.Id);

        MediatorMock.Verify(m => m.Send(It.Is<GetUserByIdCommand>(q => q.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
}