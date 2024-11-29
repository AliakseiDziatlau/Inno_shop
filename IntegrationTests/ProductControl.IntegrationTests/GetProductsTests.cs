using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using ProductControl.Core.Entities;

namespace IntegrationTests.ProductControl.IntegrationTests;

public class GetProductsTests : IntegrationTestBase
{
    [Fact]
    public async Task GetProducts_ShouldFilterAndReturnCorrectResults()
    {
        /*Test data*/
        var userRegisterRequest = new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };
    
        /*Test data*/
        var product1 = new
        {
            Name = "Product One",
            Description = "Description for Product One",
            Price = 50.0,
            IsAvailable = true
        };
    
        /*Test data*/
        var product2 = new
        {
            Name = "Product Two",
            Description = "Description for Product Two",
            Price = 150.0,
            IsAvailable = false
        };
    
        /*Test data*/
        var product3 = new
        {
            Name = "Another Product",
            Description = "Another Description",
            Price = 300.0,
            IsAvailable = true
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
        
        var productResponse1 = await userProductClient.PostAsJsonAsync("/api/products", product1);
        productResponse1.EnsureSuccessStatusCode();
    
        var productResponse2 = await userProductClient.PostAsJsonAsync("/api/products", product2);
        productResponse2.EnsureSuccessStatusCode();
    
        var productResponse3 = await userProductClient.PostAsJsonAsync("/api/products", product3);
        productResponse3.EnsureSuccessStatusCode();
        
        var searchByNameResponse = await userProductClient.GetAsync("/api/products?name=Product");
        searchByNameResponse.EnsureSuccessStatusCode();
        var searchByNameResult = await searchByNameResponse.Content.ReadFromJsonAsync<List<Product>>();
        searchByNameResult.Should().NotBeNull();
        searchByNameResult.Count.Should().Be(3);
        
        var searchByMinPriceResponse = await userProductClient.GetAsync("/api/products?minPrice=100");
        searchByMinPriceResponse.EnsureSuccessStatusCode();
        var searchByMinPriceResult = await searchByMinPriceResponse.Content.ReadFromJsonAsync<List<Product>>();
        searchByMinPriceResult.Should().NotBeNull();
        searchByMinPriceResult.Count.Should().Be(2); 
        
        var searchByAvailabilityResponse = await userProductClient.GetAsync("/api/products?isAvailable=true");
        searchByAvailabilityResponse.EnsureSuccessStatusCode();
        var searchByAvailabilityResult = await searchByAvailabilityResponse.Content.ReadFromJsonAsync<List<Product>>();
        searchByAvailabilityResult.Should().NotBeNull();
        searchByAvailabilityResult.Count.Should().Be(2); 
    }
}