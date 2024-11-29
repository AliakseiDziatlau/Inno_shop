using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace IntegrationTests.UserControl.IntegrationTests;

public class GetUserByIdTests : IntegrationTestBase
{
     [Fact]
    public async Task GetUserById_ShouldReturnUnauthorized_WhenTokenIsMissing()
    {
        var response = await UserClient.GetAsync("/api/users/1");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized); 
    }
    
    [Fact]
    public async Task GetUserById_ShouldReturnUnauthorized_WhenTokenIsInvalid()
    {
        var invalidToken = "InvalidJwtToken";
        using var UserClient = CreateNewUserClient();
        UserClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidToken);
        var response = await UserClient.GetAsync("/api/users/1");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized); 
    }
    
    [Fact]
    public async Task GetUserById_ShouldReturnForbidden_WhenUserDoesNotHaveAccess()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };
        
        var registerResponse = await UserClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();
    
        var confirmResponse = await UserClient.GetAsync($"/api/auths/confirm-email?token={await GetConfirmationTokenAsync(registerRequest.Email)}");
        confirmResponse.EnsureSuccessStatusCode();
    
        var loginRequest = new
        {
            Email = "testuser@example.com",
            Password = "TestPassword123"
        };
    
        var loginResponse = await UserClient.PostAsJsonAsync("/api/auths/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
    
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginJson = JsonDocument.Parse(loginContent).RootElement;
        var token = loginJson.GetProperty("token").GetString();
    
        var UserClientWithToken = CreateNewUserClient();
        UserClientWithToken.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await UserClientWithToken.GetAsync("/api/users/9999");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden); 
    }
    
    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenTokenIsValidAndUserIsAdmin()
    {
        /*Test data*/
        var adminRegisterRequest = new
        {
            Name = "Admin User",
            Email = "admin@example.com",
            Password = "AdminPassword123",
            Role = "Admin"
        };
    
        /*Test data*/
        var userRegisterRequest = new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };
        
        var adminRegisterResponse = await UserClient.PostAsJsonAsync("/api/auths/register", adminRegisterRequest);
        adminRegisterResponse.EnsureSuccessStatusCode();
    
        var adminConfirmResponse = await UserClient.GetAsync($"/api/auths/confirm-email?token={await GetConfirmationTokenAsync(adminRegisterRequest.Email)}");
        adminConfirmResponse.EnsureSuccessStatusCode();
    
        var adminLoginRequest = new
        {
            Email = adminRegisterRequest.Email,
            Password = adminRegisterRequest.Password
        };
    
        var adminLoginResponse = await UserClient.PostAsJsonAsync("/api/auths/login", adminLoginRequest);
        adminLoginResponse.EnsureSuccessStatusCode();
    
        var adminLoginContent = await adminLoginResponse.Content.ReadAsStringAsync();
        var adminLoginJson = JsonDocument.Parse(adminLoginContent).RootElement;
        var adminToken = adminLoginJson.GetProperty("token").GetString();
        
        var userRegisterResponse = await UserClient.PostAsJsonAsync("/api/auths/register", userRegisterRequest);
        userRegisterResponse.EnsureSuccessStatusCode();
    
        var userConfirmResponse = await UserClient.GetAsync($"/api/auths/confirm-email?token={await GetConfirmationTokenAsync(userRegisterRequest.Email)}");
        userConfirmResponse.EnsureSuccessStatusCode();
        var userId = await GetUserIdFromDatabaseAsync(userRegisterRequest.Email);
    
        var UserClientWithAdminToken = CreateNewUserClient();
        UserClientWithAdminToken.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        
        var response = await UserClientWithAdminToken.GetAsync($"/api/users/{userId}");
        response.EnsureSuccessStatusCode();
    
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrWhiteSpace();
    
        var user = JsonDocument.Parse(responseContent).RootElement;
        user.GetProperty("id").GetInt32().Should().Be(userId);
        user.GetProperty("email").GetString().Should().Be(userRegisterRequest.Email);
        user.GetProperty("name").GetString().Should().Be(userRegisterRequest.Name);
        user.GetProperty("role").GetString().Should().Be(userRegisterRequest.Role);
    }
}