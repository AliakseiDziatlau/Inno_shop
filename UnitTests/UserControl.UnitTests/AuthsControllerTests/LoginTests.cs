using Microsoft.AspNetCore.Mvc;
using Moq;
using UserControl.Application.Commands.AuthsControllerCommands;
using UserControl.Application.DTOs;

namespace UserControlTests.UserControl.UnitTests.AuthsControllerTests;

public class LoginTests : AuthsControllerTestsBase
{
    [Fact]
        public async Task Login_ReturnsOkResult_WithValidToken_WhenLoginIsSuccessful()
        {
            var requestDto = new LoginCommand
            {
                Email = "testuser@example.com",
                Password = "Test@1234"
            };

            var expectedResponse = new LoginResponseDto
            {
                Token = "ValidJwtToken"
            };

            MediatorMock.Setup(m => m.Send(requestDto, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedResponse);

            var result = await Controller.Login(requestDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<LoginResponseDto>(okResult.Value);

            Assert.Equal(expectedResponse.Token, returnedResponse.Token);
            MediatorMock.Verify(m => m.Send(requestDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Login_ThrowsUnauthorizedAccessException_WhenCredentialsAreInvalid()
        {
            var requestDto = new LoginCommand
            {
                Email = "testuser@example.com",
                Password = "WrongPassword"
            };

            MediatorMock.Setup(m => m.Send(requestDto, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new UnauthorizedAccessException("Invalid login credentials."));

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                Controller.Login(requestDto));

            Assert.Equal("Invalid login credentials.", exception.Message);
            MediatorMock.Verify(m => m.Send(requestDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Login_ThrowsUnauthorizedAccessException_WhenAccountIsNotConfirmed()
        {
            var requestDto = new LoginCommand
            {
                Email = "testuser@example.com",
                Password = "Test@1234"
            };

            MediatorMock.Setup(m => m.Send(requestDto, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new UnauthorizedAccessException("Account not confirmed."));

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                Controller.Login(requestDto));

            Assert.Equal("Account not confirmed.", exception.Message);
            MediatorMock.Verify(m => m.Send(requestDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Login_ThrowsException_WhenUnexpectedErrorOccurs()
        {
            var requestDto = new LoginCommand
            {
                Email = "testuser@example.com",
                Password = "Test@1234"
            };

            MediatorMock.Setup(m => m.Send(requestDto, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Unexpected error occurred."));

            var exception = await Assert.ThrowsAsync<Exception>(() =>
                Controller.Login(requestDto));

            Assert.Equal("Unexpected error occurred.", exception.Message);
            MediatorMock.Verify(m => m.Send(requestDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Login_ThrowsArgumentException_WhenEmailIsEmpty()
        {
            var requestDto = new LoginCommand
            {
                Email = "",
                Password = "Test@1234"
            };

            MediatorMock.Setup(m => m.Send(requestDto, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new ArgumentException("Email cannot be empty."));

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                Controller.Login(requestDto));

            Assert.Equal("Email cannot be empty.", exception.Message);
            MediatorMock.Verify(m => m.Send(requestDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Login_ThrowsArgumentException_WhenPasswordIsEmpty()
        {
            var requestDto = new LoginCommand
            {
                Email = "testuser@example.com",
                Password = ""
            };

            MediatorMock.Setup(m => m.Send(requestDto, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new ArgumentException("Password cannot be empty."));

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                Controller.Login(requestDto));

            Assert.Equal("Password cannot be empty.", exception.Message);
            MediatorMock.Verify(m => m.Send(requestDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Login_ThrowsArgumentException_WhenEmailIsInvalid()
        {
            var requestDto = new LoginCommand
            {
                Email = "invalid-email",
                Password = "Test@1234"
            };

            MediatorMock.Setup(m => m.Send(requestDto, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new ArgumentException("Invalid email format."));

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                Controller.Login(requestDto));

            Assert.Equal("Invalid email format.", exception.Message);
            MediatorMock.Verify(m => m.Send(requestDto, It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task Login_ThrowsKeyNotFoundException_WhenAccountIsDeleted()
        {
            var requestDto = new LoginCommand
            {
                Email = "deleteduser@example.com",
                Password = "Test@1234"
            };

            MediatorMock.Setup(m => m.Send(requestDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new KeyNotFoundException("Account has been deleted."));

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => Controller.Login(requestDto));

            Assert.Equal("Account has been deleted.", exception.Message);
            MediatorMock.Verify(m => m.Send(requestDto, It.IsAny<CancellationToken>()), Times.Once);
        }
}

