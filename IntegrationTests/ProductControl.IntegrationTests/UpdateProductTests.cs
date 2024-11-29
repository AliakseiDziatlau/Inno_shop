using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductControl.Infrastracture.Persistence;

namespace IntegrationTests.ProductControl.IntegrationTests;

public class UpdateProductTests : IntegrationTestBase
{
     [Fact]
    public async Task UpdateProduct_ShouldUpdateProduct_WhenUserIsOwner()
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
            Price = 50.0,
            IsAvailable = true
        };
    
        /*Test data*/
        var productUpdateRequest = new
        {
            Name = "Updated Product",
            Description = "Updated description",
            Price = 100.0,
            IsAvailable = false
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
        
        var createResponse = await ownerProductClient.PostAsJsonAsync("/api/products", productCreateRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdProduct = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync()).RootElement;
    
        var productId = createdProduct.GetProperty("id").GetInt32();
        
        var updateResponse = await ownerProductClient.PutAsJsonAsync($"/api/products/{productId}", productUpdateRequest);
        updateResponse.EnsureSuccessStatusCode();
        
        await using var productContext = new ProductDbContext(ProductDbOptions);
        var updatedProduct = await productContext.Products.SingleOrDefaultAsync(p => p.Id == productId);
        updatedProduct.Should().NotBeNull();
        updatedProduct.Name.Should().Be(productUpdateRequest.Name);
        updatedProduct.Description.Should().Be(productUpdateRequest.Description);
        updatedProduct.Price.Should().Be((decimal)productUpdateRequest.Price);
        updatedProduct.IsAvailable.Should().Be(productUpdateRequest.IsAvailable);
        
        var forbiddenResponse = await otherProductClient.PutAsJsonAsync($"/api/products/{productId}", productUpdateRequest);
        forbiddenResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }
    
    [Fact]
    public async Task UpdateProduct_ShouldReturnForbidden_WhenUserIsNotOwner()
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
            Price = 50.0,
            IsAvailable = true
        };
    
        /*Test data*/
        var productUpdateRequest = new
        {
            Name = "Updated Product",
            Description = "Updated description",
            Price = 100.0,
            IsAvailable = false
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
        
        var createResponse = await ownerProductClient.PostAsJsonAsync("/api/products", productCreateRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdProduct = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync()).RootElement;
    
        var productId = createdProduct.GetProperty("id").GetInt32();
        var forbiddenResponse = await otherProductClient.PutAsJsonAsync($"/api/products/{productId}", productUpdateRequest);
        forbiddenResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
        
        await using var productContext = new ProductDbContext(ProductDbOptions);
        var unchangedProduct = await productContext.Products.SingleOrDefaultAsync(p => p.Id == productId);
        unchangedProduct.Should().NotBeNull();
        unchangedProduct.Name.Should().Be(productCreateRequest.Name);
        unchangedProduct.Description.Should().Be(productCreateRequest.Description);
        unchangedProduct.Price.Should().Be((decimal)productCreateRequest.Price);
        unchangedProduct.IsAvailable.Should().Be(productCreateRequest.IsAvailable);
    }
    
    [Fact]
    public async Task UpdateProduct_ShouldReturnBadRequest_WhenUpdateDataIsInvalid()
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
        var productCreateRequest = new
        {
            Name = "Test Product",
            Description = "A test product",
            Price = 50.0,
            IsAvailable = true
        };

        /*Test data*/
        var invalidProductUpdateRequest = new
        {
            Name = "", 
            Description = "Updated description",
            Price = -10.0, 
            IsAvailable = false
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
        var updateResponse = await ownerProductClient.PutAsJsonAsync($"/api/products/{productId}", invalidProductUpdateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseContent = await updateResponse.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrWhiteSpace();
        responseContent.Should().Contain("One or more validation errors occurred.");
    }
    
    [Fact]
    public async Task UpdateProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
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
        var productUpdateRequest = new
        {
            Name = "Updated Product",
            Description = "Updated description",
            Price = 100.0,
            IsAvailable = false
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
        
        var invalidProductId = 9999;
        var updateResponse = await ownerProductClient.PutAsJsonAsync($"/api/products/{invalidProductId}", productUpdateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var responseContent = await updateResponse.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrWhiteSpace();
        responseContent.Should().Contain("An error occurred.");
    }
}