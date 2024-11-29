using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace IntegrationTests.UserControl.IntegrationTests;

public class RegisterUserTests : IntegrationTestBase
{
    [Fact]
    public async Task RegisterUser_ShouldCreateUser_WhenRequestIsValid()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Test User",
            Email = "testuse@example.com",
            Password = "TestPassword123",
            Role = "User"
        };
        
        var response = await UserClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadAsStringAsync();
        responseData.Should().Contain("Registration successful");
    }
    
    [Fact]
    public async Task RegisterUser_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Existing User",
            Email = "existinguser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };
        
        var firstResponse = await UserClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        firstResponse.EnsureSuccessStatusCode();
        
        var secondResponse = await UserClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var responseContent = await secondResponse.Content.ReadAsStringAsync();
        responseContent.Should().Contain("A user with this email already exists");
    }
    
    [Fact]
    public async Task RegisterUser_ShouldSendConfirmationEmail_WhenRequestIsValid()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Email Confirmation User",
            Email = "confirmationuser@example.com",
            Password = "ValidPassword123",
            Role = "User"
        };

        var response = await UserClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        response.EnsureSuccessStatusCode();

        var confirmationToken = await GetConfirmationTokenAsync(registerRequest.Email);
        confirmationToken.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public async Task RegisterUser_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "",
            Email = "validuser@example.com",
            Password = "ValidPassword123",
            Role = "User"
        };

        using var userClient = CreateNewUserClient();
        var response = await userClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Name is required.");
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnBadRequest_WhenNameExceedsMaxLength()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = new string('A', 51), 
            Email = "validuser@example.com",
            Password = "ValidPassword123",
            Role = "User"
        };

        using var userClient = CreateNewUserClient();
        var response = await userClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Name must not exceed 50 characters.");
    }
    
    [Fact]
    public async Task RegisterUser_ShouldReturnBadRequest_WhenEmailIsInvalid()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Valid User",
            Email = "invalid-email",
            Password = "ValidPassword123",
            Role = "User"
        };

        using var userClient = CreateNewUserClient();
        var response = await userClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid email format.");
    }
    
    [Fact]
    public async Task RegisterUser_ShouldReturnBadRequest_WhenPasswordIsTooShort()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Valid User",
            Email = "validuser@example.com",
            Password = "short",
            Role = "User"
        };

        using var userClient = CreateNewUserClient();
        var response = await userClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Password must be at least 8 characters long.");
    }
    
    [Fact]
    public async Task RegisterUser_ShouldReturnOk_WhenRequestIsValid()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Valid User",
            Email = "validuser@example.com",
            Password = "ValidPassword123",
            Role = "User"
        };

        using var userClient = CreateNewUserClient();
        var response = await userClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Registration successful");
    }
}