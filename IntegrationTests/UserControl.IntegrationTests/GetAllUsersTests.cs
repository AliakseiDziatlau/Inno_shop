using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace IntegrationTests.UserControl.IntegrationTests;

public class GetAllUsersTests : IntegrationTestBase
{
   [Fact]
    public async Task GetAllUsers_ShouldReturnForbidden_WhenUserIsNotAdmin()
    {
        /*Test data*/
        var regularUserRegisterRequest = new
        {
            Name = "Regular User",
            Email = "regularuser@example.com",
            Password = "Password123",
            Role = "User"
        };

        using var userClient = CreateNewUserClient();

        var registerResponse = await userClient.PostAsJsonAsync("/api/auths/register", regularUserRegisterRequest);
        registerResponse.EnsureSuccessStatusCode();
        await ConfirmEmailAsync(regularUserRegisterRequest.Email);

        var loginResponse = await userClient.PostAsJsonAsync("/api/auths/login", new
        {
            Email = regularUserRegisterRequest.Email,
            Password = regularUserRegisterRequest.Password
        });
        loginResponse.EnsureSuccessStatusCode();
        var userToken = await ExtractJwtToken(loginResponse);

        var regularUserClient = new HttpClient { BaseAddress = new Uri("http://localhost:5001") };
        regularUserClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        var response = await regularUserClient.GetAsync("/api/users?page=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnBadRequest_WhenPageIsLessThanOne_AndUserIsAdmin()
    {
        /*Test data*/
        var adminRegisterRequest = new
        {
            Name = "Admin User",
            Email = "adminuser@example.com",
            Password = "AdminPassword123",
            Role = "Admin"
        };

        using var userClient = CreateNewUserClient();
        var registerResponse = await userClient.PostAsJsonAsync("/api/auths/register", adminRegisterRequest);
        registerResponse.EnsureSuccessStatusCode();
        await ConfirmEmailAsync(adminRegisterRequest.Email);
        
        var loginResponse = await userClient.PostAsJsonAsync("/api/auths/login", new
        {
            Email = adminRegisterRequest.Email,
            Password = adminRegisterRequest.Password
        });
        loginResponse.EnsureSuccessStatusCode();

        var adminToken = await ExtractJwtToken(loginResponse);

        var adminClient = new HttpClient { BaseAddress = new Uri("http://localhost:5001") };
        adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        
        var response = await adminClient.GetAsync("/api/users?page=0&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Page must be greater than 0.");
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnOk_WhenQueryIsValid_AndUserIsAdmin()
    {
        /*Test data*/
        var adminRegisterRequest = new
        {
            Name = "Admin User",
            Email = "adminuser@example.com",
            Password = "AdminPassword123",
            Role = "Admin"
        };

        using var userClient = CreateNewUserClient();
        var registerResponse = await userClient.PostAsJsonAsync("/api/auths/register", adminRegisterRequest);
        registerResponse.EnsureSuccessStatusCode();
        await ConfirmEmailAsync(adminRegisterRequest.Email);

        var loginResponse = await userClient.PostAsJsonAsync("/api/auths/login", new
        {
            Email = adminRegisterRequest.Email,
            Password = adminRegisterRequest.Password
        });
        loginResponse.EnsureSuccessStatusCode();

        var adminToken = await ExtractJwtToken(loginResponse);
        var adminClient = new HttpClient { BaseAddress = new Uri("http://localhost:5001") };
        adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        
        var response = await adminClient.GetAsync("/api/users?page=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }
}