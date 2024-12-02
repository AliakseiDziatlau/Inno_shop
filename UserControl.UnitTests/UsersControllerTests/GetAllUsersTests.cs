using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.DTOs;

namespace UserControl.UnitTests.UsersControllerTests;

public class GetAllUsersTests : UsersControllerTestsBase
{
    [Fact]
    public async Task GetAllUsers_ReturnsOkResultWithUsers_WhenUsersExist()
    {
        var page = 1;
        var pageSize = 10;

        var users = new List<UserDto>
        {
            new UserDto { Id = 1, Name = "User 1", Email = "user1@example.com", Role = "User", IsActive = true },
            new UserDto { Id = 2, Name = "User 2", Email = "user2@example.com", Role = "Admin", IsActive = false }
        };

        MediatorMock.Setup(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var result = await Controller.GetAllUsers(page, pageSize);
        
        var actionResult = Assert.IsType<ActionResult<IEnumerable<UserDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); 
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);

        Assert.Equal(users.Count, returnedUsers.Count());
        Assert.Equal(users, returnedUsers);

        MediatorMock.Verify(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOkResultWithEmptyList_WhenNoUsersExist()
    {
        var page = 1;
        var pageSize = 10;

        MediatorMock.Setup(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserDto>());

        var result = await Controller.GetAllUsers(page, pageSize);
        
        var actionResult = Assert.IsType<ActionResult<IEnumerable<UserDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); 
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);

        Assert.Empty(returnedUsers);

        MediatorMock.Verify(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllUsers_ThrowsArgumentException_WhenPageIsInvalid()
    {
        var invalidPage = -1;
        var pageSize = 10;

        MediatorMock.Setup(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == invalidPage && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Page number must be greater than 0."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.GetAllUsers(invalidPage, pageSize));

        Assert.Equal("Page number must be greater than 0.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == invalidPage && q.PageSize == pageSize), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllUsers_ThrowsArgumentException_WhenPageSizeIsInvalid()
    {
        var page = 1;
        var invalidPageSize = 0;

        MediatorMock.Setup(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == invalidPageSize), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Page size must be greater than 0."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.GetAllUsers(page, invalidPageSize));

        Assert.Equal("Page size must be greater than 0.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == invalidPageSize), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllUsers_ThrowsException_WhenUnexpectedErrorOccurs()
    {
        var page = 1;
        var pageSize = 10;

        MediatorMock.Setup(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("An unexpected error occurred."));

        var exception = await Assert.ThrowsAsync<Exception>(() => Controller.GetAllUsers(page, pageSize));
        Assert.Equal("An unexpected error occurred.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOkResultWithUsers_WhenPageAndPageSizeAreBoundaryValues()
    {
        var page = 1;
        var pageSize = 1000;
        var users = Enumerable.Range(1, 10).Select(i => new UserDto
        {
            Id = i,
            Name = $"User {i}",
            Email = $"user{i}@example.com",
            Role = "User",
            IsActive = true
        }).ToList();

        MediatorMock.Setup(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var result = await Controller.GetAllUsers(page, pageSize);
        
        var actionResult = Assert.IsType<ActionResult<IEnumerable<UserDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);

        Assert.Equal(users.Count, returnedUsers.Count());
        Assert.Equal(users, returnedUsers);

        MediatorMock.Verify(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsEmptyList_WhenPageExceedsTotalPages()
    {
        var page = 100;
        var pageSize = 10;

        MediatorMock.Setup(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserDto>());

        var result = await Controller.GetAllUsers(page, pageSize);
        
        var actionResult = Assert.IsType<ActionResult<IEnumerable<UserDto>>>(result);
        
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
        Assert.Empty(returnedUsers);

        MediatorMock.Verify(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GetAllUsers_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthorized()
    {
        var page = 1;
        var pageSize = 10;

        MediatorMock.Setup(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("User is not authorized."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.GetAllUsers(page, pageSize));

        Assert.Equal("User is not authorized.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GetAllUsers_ThrowsDbUpdateException_WhenDatabaseErrorOccurs()
    {
        var page = 1;
        var pageSize = 10;

        MediatorMock.Setup(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Database error occurred."));

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => Controller.GetAllUsers(page, pageSize));

        Assert.Equal("Database error occurred.", exception.Message);

        MediatorMock.Verify(m => m.Send(It.Is<GetAllUsersCommand>(q => q.Page == page && q.PageSize == pageSize), It.IsAny<CancellationToken>()), Times.Once);
    }
}