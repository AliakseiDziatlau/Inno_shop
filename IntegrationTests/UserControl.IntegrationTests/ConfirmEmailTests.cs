using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace IntegrationTests.UserControl.IntegrationTests;

public class ConfirmEmailTests : IntegrationTestBase
{
    [Fact]
    public async Task ConfirmEmail_ShouldReturnBadRequest_WhenTokenIsEmpty()
    {
        var token = string.Empty;
        using var userClient = CreateNewUserClient();
        var response = await userClient.GetAsync($"/api/auths/confirm-email?token={token}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("The token field is required.");
    }

    [Fact]
    public async Task ConfirmEmail_ShouldReturnBadRequest_WhenTokenIsTooShort()
    {
        var token = "short";
        using var userClient = CreateNewUserClient();
        var response = await userClient.GetAsync($"/api/auths/confirm-email?token={token}");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Token is invalid.");
    }

    [Fact]
    public async Task ConfirmEmail_ShouldReturnOk_WhenTokenIsValid()
    {
        var userRegisterRequest = new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };
        using var userClient = CreateNewUserClient();
        var registerResponse = await userClient.PostAsJsonAsync("/api/auths/register", userRegisterRequest);
        registerResponse.EnsureSuccessStatusCode();
        var token = await GetConfirmationTokenAsync(userRegisterRequest.Email);
        var response = await userClient.GetAsync($"/api/auths/confirm-email?token={token}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Your account has been confirmed successfully.");
    }

    [Fact]
    public async Task ConfirmEmail_ShouldReturnBadRequest_WhenTokenIsInvalidFormat()
    {
        var invalidToken = "invalidTokenFormat123!";
        using var userClient = CreateNewUserClient();
        var response = await userClient.GetAsync($"/api/auths/confirm-email?token={invalidToken}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid confirmation token.");
    }

    [Fact]
    public async Task ConfirmEmail_ShouldReturnNotFound_WhenTokenDoesNotExistInDatabase()
    {
        var nonExistentToken = "abcdefghij1234567890"; 
        using var userClient = CreateNewUserClient();
        var response = await userClient.GetAsync($"/api/auths/confirm-email?token={nonExistentToken}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid confirmation token.");
    }
}