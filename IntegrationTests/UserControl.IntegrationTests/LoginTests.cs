using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserControl.Infrastructure.Persistence;

namespace IntegrationTests.UserControl.IntegrationTests;

public class LoginTests : IntegrationTestBase
{
    [Fact]
    public async Task Login_ShouldReturnJwtToken_WhenCredentialsAreValid()
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
        var loginRequest = new
        {
            Email = "testuser@example.com",
            Password = "TestPassword123"
        };

        using var userClient = CreateNewUserClient();
        
        var registerResponse = await userClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();
        
        var confirmationToken = await GetConfirmationTokenAsync(registerRequest.Email);
        var confirmResponse = await userClient.GetAsync($"/api/auths/confirm-email?token={confirmationToken}");
        confirmResponse.EnsureSuccessStatusCode();
        
        var loginResponse = await userClient.PostAsJsonAsync("/api/auths/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var responseContent = await loginResponse.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrWhiteSpace();

        var responseJson = JsonDocument.Parse(responseContent).RootElement;
        responseJson.TryGetProperty("token", out var jwtToken).Should().BeTrue();

        jwtToken.GetString().Should().NotBeNullOrWhiteSpace();
        ValidateJwtToken(jwtToken.GetString());
    }
    
    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        /*Test data*/
        var invalidLoginRequest = new
        {
            Email = "nonexistentuser@example.com", //no such email in the DB
            Password = "WrongPassword123"          //no such password in the DB
        };

        using var UserClient = CreateNewUserClient();
        var loginResponse = await UserClient.PostAsJsonAsync("/api/auths/login", invalidLoginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var responseContent = await loginResponse.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrWhiteSpace();
        
        var responseJson = JsonDocument.Parse(responseContent).RootElement;
        responseJson.TryGetProperty("status", out var status).Should().BeTrue();
        status.GetInt32().Should().Be(401);

        responseJson.TryGetProperty("detail", out var detail).Should().BeTrue();
        detail.GetString().Should().Contain("Invalid login credentials");
    }
    
    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenAccountIsNotConfirmed()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Test User",
            Email = "unconfirmed@example.com",
            Password = "Password123",
            Role = "User"
        };

        /*Test data*/
        var loginRequest = new
        {
            Email = "unconfirmed@example.com",
            Password = "Password123"
        };

        using var userClient = CreateNewUserClient();
    
        var registerResponse = await userClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await userClient.PostAsJsonAsync("/api/auths/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var responseContent = await loginResponse.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrWhiteSpace();
    
        var responseJson = JsonDocument.Parse(responseContent).RootElement;
        responseJson.TryGetProperty("detail", out var detail).Should().BeTrue();
        detail.GetString().Should().Contain("Account not confirmed");
    }
    
    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "CorrectPassword123",
            Role = "User"
        };

        /*Test data*/
        var loginRequest = new
        {
            Email = "testuser@example.com",
            Password = "WrongPassword123"
        };

        using var userClient = CreateNewUserClient();
    
        var registerResponse = await userClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var confirmationToken = await GetConfirmationTokenAsync(registerRequest.Email);
        var confirmResponse = await userClient.GetAsync($"/api/auths/confirm-email?token={confirmationToken}");
        confirmResponse.EnsureSuccessStatusCode();

        var loginResponse = await userClient.PostAsJsonAsync("/api/auths/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var responseContent = await loginResponse.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrWhiteSpace();

        var responseJson = JsonDocument.Parse(responseContent).RootElement;
        responseJson.TryGetProperty("detail", out var detail).Should().BeTrue();
        detail.GetString().Should().Contain("Invalid login credentials");
    }
    
    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserIsDeactivated()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Deactivated User",
            Email = "deactivated@example.com",
            Password = "DeactivatedPassword123",
            Role = "User"
        };

        /*Test data*/
        var loginRequest = new
        {
            Email = "deactivated@example.com",
            Password = "DeactivatedPassword123"
        };

        using var userClient = CreateNewUserClient();
        
        var registerResponse = await userClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();
        
        var confirmationToken = await GetConfirmationTokenAsync(registerRequest.Email);
        var confirmResponse = await userClient.GetAsync($"/api/auths/confirm-email?token={confirmationToken}");
        confirmResponse.EnsureSuccessStatusCode();
        
        await using var userContext = new UserDbContext(UserDbOptions);
        var user = await userContext.user.SingleOrDefaultAsync(u => u.Email == registerRequest.Email);
        user.Should().NotBeNull();
        user.IsActive = false; 
        await userContext.SaveChangesAsync();
        
        var loginResponse = await userClient.PostAsJsonAsync("/api/auths/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var responseContent = await loginResponse.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrWhiteSpace();

        var responseJson = JsonDocument.Parse(responseContent).RootElement;
        responseJson.TryGetProperty("detail", out var detail).Should().BeTrue();
        detail.GetString().Should().Contain("Your account has been deactivated. Please contact support.");
    }
    
    private void ValidateJwtToken(string jwtToken)
    {
        var key = "YourSuperSecretKey"; 
        var issuer = "YourIssuer";
        var audience = "YourAudience";

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
        
        tokenHandler.ValidateToken(jwtToken, validationParameters, out _); 
    }
}