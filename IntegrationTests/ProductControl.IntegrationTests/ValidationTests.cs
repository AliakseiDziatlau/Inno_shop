using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace IntegrationTests.ProductControl.IntegrationTests;

public class ValidationTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateProduct_ShouldReturnValidationErrors_WhenDataIsInvalid()
    {
        /*Test data*/
        var userRegisterRequest = new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };

        /*Data for comparison*/
        ProductRequest productRequest1 = new ProductRequest { Name = "", Description = "Valid Description", Price = 100.0, IsAvailable = true };
        ProductRequest productRequest2 = new ProductRequest { Name = "Valid Name", Description = "Valid Description", Price = -10.0, IsAvailable = true };
        ProductRequest productRequest3 = new ProductRequest { Name = "Valid Name", Description = "Valid Description", Price = 100.0, IsAvailable = null };

        var invalidProductRequests = new List<ProductRequest>
        {
            productRequest1,
            productRequest2,
            productRequest3
        };

        using var UserClient = CreateNewUserClient();
        using var ProductClient = CreateNewProductClient();
        
        var userRegisterResponse = await UserClient.PostAsJsonAsync("/api/auths/register", userRegisterRequest);
        userRegisterResponse.EnsureSuccessStatusCode();

        var userConfirmResponse = await UserClient.GetAsync($"/api/auths/confirm-email?token={await GetConfirmationTokenAsync(userRegisterRequest.Email)}");
        userConfirmResponse.EnsureSuccessStatusCode();

        var userLoginResponse = await UserClient.PostAsJsonAsync("/api/auths/login", new
        {
            Email = userRegisterRequest.Email,
            Password = userRegisterRequest.Password
        });
        userLoginResponse.EnsureSuccessStatusCode();
        var userToken = await ExtractJwtToken(userLoginResponse);
        var userProductClient = new HttpClient { BaseAddress = new Uri("http://localhost:5002") };
        userProductClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        foreach (var invalidRequest in invalidProductRequests)
        {
            var response = await userProductClient.PostAsJsonAsync("/api/products", invalidRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrWhiteSpace();

            /*verify validation errors in the response*/
            var validationErrors = JsonDocument.Parse(responseContent).RootElement;
            validationErrors.TryGetProperty("errors", out var topLevelErrors).Should().BeTrue();

            /*check specific validation messages*/
            if (string.IsNullOrWhiteSpace(invalidRequest.Name))
            {
                topLevelErrors.GetProperty("Name").ToString().Should().Contain("Product name is required.");
            }
            if (!string.IsNullOrWhiteSpace(invalidRequest.Name) && invalidRequest.Name.Length > 100)
            {
                topLevelErrors.GetProperty("Name").ToString().Should().Contain("Product name cannot exceed 100 characters.");
            }
            if (!string.IsNullOrWhiteSpace(invalidRequest.Description) && invalidRequest.Description.Length > 500)
            {
                topLevelErrors.GetProperty("Description").ToString().Should().Contain("Description cannot exceed 500 characters.");
            }
            if (invalidRequest.Price <= 0)
            {
                topLevelErrors.GetProperty("Price").ToString().Should().Contain("Price must be greater than zero.");
            }
            if (!invalidRequest.IsAvailable.HasValue)
            {
                validationErrors.ToString().Should().Contain("The command field is required.");
            }
        }
    }
    
    private class ProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double? Price { get; set; }
        public bool? IsAvailable { get; set; }
    }
}