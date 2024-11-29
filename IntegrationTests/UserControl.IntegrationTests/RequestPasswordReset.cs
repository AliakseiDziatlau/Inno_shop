using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserControl.Infrastructure.Persistence;

namespace IntegrationTests.UserControl.IntegrationTests;

public class RequestPasswordReset : IntegrationTestBase
{
    [Fact]
    public async Task RequestPasswordReset_ShouldGenerateResetToken_WhenEmailIsValid()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };

        /*Test data*/
        var passwordResetRequest = new
        {
            Email = "testuser@example.com"
        };

        using var UserClient = CreateNewUserClient();
        
        var registerResponse = await UserClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();
        
        var confirmResponse = await UserClient.GetAsync($"/api/auths/confirm-email?token={await GetConfirmationTokenAsync(registerRequest.Email)}");
        confirmResponse.EnsureSuccessStatusCode();
        
        var passwordResetResponse = await UserClient.PostAsJsonAsync("/api/auths/request-password-reset", passwordResetRequest);
        passwordResetResponse.EnsureSuccessStatusCode();
        var responseContent = await passwordResetResponse.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Password reset link sent to your email.");
        
        await using var userContext = new UserDbContext(UserDbOptions);
        var user = await userContext.user.SingleOrDefaultAsync(u => u.Email == passwordResetRequest.Email);

        user.Should().NotBeNull();
        user.PasswordResetToken.Should().NotBeNullOrWhiteSpace();
        user.PasswordResetTokenExpiryTime.Should().BeAfter(DateTime.UtcNow);
    }
    
    [Fact]
    public async Task RequestPasswordReset_ShouldReturnNotFound_WhenEmailIsNotValid()
    {
        /*Test data*/
        var passwordResetRequest = new
        {
            Email = "nonexistentuser@example.com" /*no such email in the DB*/
        };

        using var UserClient = CreateNewUserClient();
        var passwordResetResponse = await UserClient.PostAsJsonAsync("/api/auths/request-password-reset", passwordResetRequest);
        passwordResetResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var responseContent = await passwordResetResponse.Content.ReadAsStringAsync();
        responseContent.Should().Contain("User with the specified email was not found.");
    }
    
    [Fact]
    public async Task RequestPasswordReset_ShouldReturnBadRequest_WhenEmailIsEmpty()
    {
        /*Test data*/
        var request = new
        {
            Email = ""
        };

        using var userClient = CreateNewUserClient();
        var response = await userClient.PostAsJsonAsync("/api/auths/request-password-reset", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Email is required.");
    }
    
    [Fact]
    public async Task RequestPasswordReset_ShouldReturnBadRequest_WhenEmailIsInvalid()
    {
        /*Test data*/
        var request = new
        {
            Email = "invalid-email"
        };

        using var userClient = CreateNewUserClient();
        var response = await userClient.PostAsJsonAsync("/api/auths/request-password-reset", request);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid email format.");
    }
    
    [Fact]
    public async Task RequestPasswordReset_ShouldReturnOk_WhenEmailIsValid()
    {
        /*Test data*/
        var userRegisterRequest = new
        {
            Name = "Test User",
            Email = "validuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };

        var request = new
        {
            Email = userRegisterRequest.Email
        };

        using var userClient = CreateNewUserClient();
        var registerResponse = await userClient.PostAsJsonAsync("/api/auths/register", userRegisterRequest);
        registerResponse.EnsureSuccessStatusCode();
        await ConfirmEmailAsync(userRegisterRequest.Email);
        var response = await userClient.PostAsJsonAsync("/api/auths/request-password-reset", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Password reset link sent to your email.");
    }
}