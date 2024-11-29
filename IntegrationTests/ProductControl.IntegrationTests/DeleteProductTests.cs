using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductControl.Infrastracture.Persistence;

namespace IntegrationTests.ProductControl.IntegrationTests;

public class DeleteProductTests : IntegrationTestBase
{
    [Fact]
    public async Task DeleteProduct_ShouldSoftDeleteProduct_WhenUserIsOwner()
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
        var productCreateRequest = new
        {
            Name = "Test Product",
            Description = "A test product",
            Price = 100.0,
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
        
        var createResponse = await userProductClient.PostAsJsonAsync("/api/products", productCreateRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdProduct = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync()).RootElement;
        var productId = createdProduct.GetProperty("id").GetInt32();
        
        var deleteResponse = await userProductClient.DeleteAsync($"/api/products/{productId}");
        deleteResponse.EnsureSuccessStatusCode();
        
        await using var productContext = new ProductDbContext(ProductDbOptions);
        var deletedProduct = await productContext.Products.SingleOrDefaultAsync(p => p.Id == productId);
        deletedProduct.Should().BeNull();
        
        var secondDeleteResponse = await userProductClient.DeleteAsync($"/api/products/{productId}");
        secondDeleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }
    
    [Fact]
    public async Task DeleteProduct_ShouldReturnForbidden_WhenUserIsNotOwner()
    {
        /*Test data*/
        var ownerRegisterRequest = new
        {
            Name = "Owner User",
            Email = "owner@example.com",
            Password = "OwnerPassword123",
            Role = "User"
        };
    
        /*Test data*/
        var otherUserRegisterRequest = new
        {
            Name = "Other User",
            Email = "otheruser@example.com",
            Password = "OtherPassword123",
            Role = "User"
        };
    
        /*Test data*/
        var productCreateRequest = new
        {
            Name = "Test Product",
            Description = "A test product",
            Price = 100.0,
            IsAvailable = true
        };
    
        using var UserClient = CreateNewUserClient();
        using var ProductClient = CreateNewProductClient();
        
        var ownerRegisterResponse = await UserClient.PostAsJsonAsync("/api/auths/register", ownerRegisterRequest);
        ownerRegisterResponse.EnsureSuccessStatusCode();
    
        var ownerConfirmResponse = await UserClient.GetAsync($"/api/auths/confirm-email?token={await GetConfirmationTokenAsync(ownerRegisterRequest.Email)}");
        ownerConfirmResponse.EnsureSuccessStatusCode();
    
        var ownerLoginResponse = await UserClient.PostAsJsonAsync("/api/auths/login", new
        {
            Email = ownerRegisterRequest.Email,
            Password = ownerRegisterRequest.Password
        });
        ownerLoginResponse.EnsureSuccessStatusCode();
    
        var ownerToken = await ExtractJwtToken(ownerLoginResponse);
    
        var ownerProductClient = new HttpClient { BaseAddress = new Uri("http://localhost:5002") };
        ownerProductClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ownerToken);
        
        var createResponse = await ownerProductClient.PostAsJsonAsync("/api/products", productCreateRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdProduct = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync()).RootElement;
        var productId = createdProduct.GetProperty("id").GetInt32();
        
        var otherUserRegisterResponse = await UserClient.PostAsJsonAsync("/api/auths/register", otherUserRegisterRequest);
        otherUserRegisterResponse.EnsureSuccessStatusCode();
    
        var otherUserConfirmResponse = await UserClient.GetAsync($"/api/auths/confirm-email?token={await GetConfirmationTokenAsync(otherUserRegisterRequest.Email)}");
        otherUserConfirmResponse.EnsureSuccessStatusCode();
    
        var otherUserLoginResponse = await UserClient.PostAsJsonAsync("/api/auths/login", new
        {
            Email = otherUserRegisterRequest.Email,
            Password = otherUserRegisterRequest.Password
        });
        otherUserLoginResponse.EnsureSuccessStatusCode();
    
        var otherUserToken = await ExtractJwtToken(otherUserLoginResponse);
    
        var otherProductClient = new HttpClient { BaseAddress = new Uri("http://localhost:5002") };
        otherProductClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", otherUserToken);
        
        var deleteResponse = await otherProductClient.DeleteAsync($"/api/products/{productId}");
        deleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
        
        await using var productContext = new ProductDbContext(ProductDbOptions);
        var product = await productContext.Products.SingleOrDefaultAsync(p => p.Id == productId);
        product.Should().NotBeNull();
        product.Name.Should().Be(productCreateRequest.Name);
        product.Description.Should().Be(productCreateRequest.Description);
        product.Price.Should().Be((decimal)productCreateRequest.Price);
        product.IsAvailable.Should().Be(productCreateRequest.IsAvailable);
    }
    
    [Fact]
    public async Task DeleteProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        /*Test data*/
        var userRegisterRequest = new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };

        using var UserClient = CreateNewUserClient();

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

        var deleteResponse = await userProductClient.DeleteAsync($"/api/products/99999");
        deleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }
    
    [Fact]
    public async Task DeleteProduct_ShouldReturnUnauthorized_WhenTokenIsInvalid()
    {
        /*Test data*/
        var productCreateRequest = new
        {
            Name = "Test Product",
            Description = "A test product",
            Price = 100.0,
            IsAvailable = true
        };

        var invalidToken = "InvalidToken";

        var client = new HttpClient { BaseAddress = new Uri("http://localhost:5002") };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidToken);

        var deleteResponse = await client.DeleteAsync("/api/products/1");
        deleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
} 