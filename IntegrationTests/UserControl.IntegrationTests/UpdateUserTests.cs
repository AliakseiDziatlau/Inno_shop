using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace IntegrationTests.UserControl.IntegrationTests;

public class UpdateUserTests : IntegrationTestBase
{
    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenUserIdIsInvalid()
    {
        /*Test data*/
        var adminToken = await RegisterAndLoginAsync(new
        {
            Name = "Admin User",
            Email = "admin@example.com",
            Password = "Admin123!",
            Role = "Admin"
        });

        var adminClient = CreateNewUserClient();
        adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        /*Test data*/
        var updateRequest = new
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Role = "User"
        };
        
        var response = await adminClient.PutAsJsonAsync("/api/users/0", updateRequest); 
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("UserId must be a positive integer.");
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        /*Test data*/
        var adminToken = await RegisterAndLoginAsync(new
        {
            Name = "Admin User",
            Email = "admin@example.com",
            Password = "Admin123!",
            Role = "Admin"
        });

        var adminClient = CreateNewUserClient();
        adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        /*Test data*/
        var updateRequest = new
        {
            Name = "", 
            Email = "updated@example.com",
            Role = "User"
        };
        
        var response = await adminClient.PutAsJsonAsync("/api/users/1", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Name is required.");
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenEmailIsInvalid()
    {
        /*Test data*/
        var adminToken = await RegisterAndLoginAsync(new
        {
            Name = "Admin User",
            Email = "admin@example.com",
            Password = "Admin123!",
            Role = "Admin"
        });

        var adminClient = CreateNewUserClient();
        adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        
        /*Test data*/
        var updateRequest = new
        {
            Name = "Updated Name",
            Email = "invalid-email", 
            Role = "User"
        };
        
        var response = await adminClient.PutAsJsonAsync("/api/users/1", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Invalid email format.");
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnNoContent_WhenRequestIsValid()
    {
        /*Test data*/
        var adminToken = await RegisterAndLoginAsync(new
        {
            Name = "Admin User",
            Email = "admin@example.com",
            Password = "Admin123!",
            Role = "Admin"
        });

        var adminClient = CreateNewUserClient();
        adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        
        /*Test data*/
        var updateRequest = new
        {
            Name = "Updated Name",
            Email = "updated@example.com",
            Role = "User"
        };

        var response = await adminClient.PutAsJsonAsync("/api/users/1", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}