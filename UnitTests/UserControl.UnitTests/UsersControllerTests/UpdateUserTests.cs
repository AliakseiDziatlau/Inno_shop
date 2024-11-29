using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserControl.Application.Commands.UsersControllerCommands;
using UserControl.Application.DTOs;

namespace UserControlTests.UserControl.UnitTests.UsersControllerTests;

public class UpdateUserTests : UsersControllerTestsBase
{
    [Fact]
    public async Task UpdateUser_ReturnsNoContent_WhenUpdateIsSuccessful()
    {
        var userId = 1;
        var updateRequest = new UpdateUserRequestDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Role = "User"
        };

        var command = new UpdateUserCommand(userId, updateRequest.Name, updateRequest.Email, updateRequest.Role);

        MediatorMock.Setup(m => m.Send(It.Is<UpdateUserCommand>(c => 
            c.UserId == userId &&
            c.Name == updateRequest.Name &&
            c.Email == updateRequest.Email &&
            c.Role == updateRequest.Role), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.UpdateUser(userId, updateRequest);
        
        Assert.IsType<NoContentResult>(result);
        MediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ThrowsKeyNotFoundException_WhenUserDoesNotExist()
    {
        var userId = 1;
        var updateRequest = new UpdateUserRequestDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException($"User with ID {userId} not found."));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.UpdateUser(userId, updateRequest));

        Assert.Equal($"User with ID {userId} not found.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ThrowsArgumentException_WhenRequestIsInvalid()
    {
        var userId = 1;
        var updateRequest = new UpdateUserRequestDto
        {
            Name = "Updated Name",
            Email = "invalid-email",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid email address."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.UpdateUser(userId, updateRequest));

        Assert.Equal("Invalid email address.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthorized()
    {
        var userId = 1;
        var updateRequest = new UpdateUserRequestDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("You are not authorized to update this user."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.UpdateUser(userId, updateRequest));

        Assert.Equal("You are not authorized to update this user.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ThrowsArgumentException_WhenUserIdIsInvalid()
    {
        var invalidUserId = 0;
        var updateRequest = new UpdateUserRequestDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("User ID must be greater than 0."));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => Controller.UpdateUser(invalidUserId, updateRequest));

        Assert.Equal("User ID must be greater than 0.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ThrowsValidationException_WhenMandatoryFieldsAreMissing()
    {
        var userId = 1;
        var updateRequest = new UpdateUserRequestDto
        {
            Name = "",
            Email = "",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("Name and Email are required."));

        var exception = await Assert.ThrowsAsync<ValidationException>(() => Controller.UpdateUser(userId, updateRequest));

        Assert.Equal("Name and Email are required.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateUser_UpdatesRoleSuccessfully()
    {
        var userId = 1;
        var updateRequest = new UpdateUserRequestDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Role = "Admin"
        };

        MediatorMock.Setup(m => m.Send(It.Is<UpdateUserCommand>(c =>
                c.UserId == userId &&
                c.Role == updateRequest.Role), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.UpdateUser(userId, updateRequest);

        Assert.IsType<NoContentResult>(result);
        MediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateUser_DoesNotChangeAnything_WhenDataIsSame()
    {
        var userId = 1;
        var updateRequest = new UpdateUserRequestDto
        {
            Name = "Same Name",
            Email = "same@example.com",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Unit.Value));

        var result = await Controller.UpdateUser(userId, updateRequest);

        Assert.IsType<NoContentResult>(result);
        MediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateUser_ThrowsValidationException_WhenNameExceedsMaxLength()
    {
        var userId = 1;
        var updateRequest = new UpdateUserRequestDto
        {
            Name = new string('A', 256),
            Email = "updated@example.com",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException("Name cannot exceed 255 characters."));

        var exception = await Assert.ThrowsAsync<ValidationException>(() => Controller.UpdateUser(userId, updateRequest));

        Assert.Equal("Name cannot exceed 255 characters.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateUser_ThrowsKeyNotFoundException_WhenUpdatingNonExistentUser()
    {
        var userId = 999; 
        var updateRequest = new UpdateUserRequestDto
        {
            Name = "Non-existent User",
            Email = "nonexistent@example.com",
            Role = "User"
        };

        MediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException($"User with ID {userId} not found."));

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.UpdateUser(userId, updateRequest));

        Assert.Equal($"User with ID {userId} not found.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateUser_ThrowsUnauthorizedAccessException_WhenUpdatingAdminRoleWithoutPermission()
    {
        var userId = 1;
        var updateRequest = new UpdateUserRequestDto
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Role = "Admin"
        };

        MediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Not authorized to assign Admin role."));

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Controller.UpdateUser(userId, updateRequest));

        Assert.Equal("Not authorized to assign Admin role.", exception.Message);
        MediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}