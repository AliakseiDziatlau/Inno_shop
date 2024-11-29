using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductControl.Infrastracture.Persistence;
using UserControl.Infrastructure.Persistence;

namespace IntegrationTests.UserControl.IntegrationTests;

public class DeactivateAndActivateUserTests : IntegrationTestBase
{
    [Fact]
    public async Task DeactivateAndReactivateUser_ShouldToggleProductVisibility()
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
        
        var adminToken = await RegisterAndLoginAsync(adminRegisterRequest);
        var userToken = await RegisterAndLoginAsync(userRegisterRequest);
        
        var userProductClient = CreateAuthorizedClient(userToken, ProductClient.BaseAddress);
        var productResponse = await userProductClient.PostAsJsonAsync("/api/products", productCreateRequest);
        productResponse.EnsureSuccessStatusCode();
        
        var userId = await GetUserIdFromDatabaseAsync(userRegisterRequest.Email);
        var adminUserClient = CreateAuthorizedClient(adminToken, UserClient.BaseAddress);
        var deactivateResponse = await adminUserClient.PutAsync($"/api/users/deactivate/{userId}", null);
        deactivateResponse.EnsureSuccessStatusCode();
        
        await using var userContext = new UserDbContext(UserDbOptions);
        var user = await userContext.user.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);
        user.Should().NotBeNull();
        user.IsActive.Should().BeFalse();
        
        await using var productContext = new ProductDbContext(ProductDbOptions);
        var products = await productContext.Products.IgnoreQueryFilters()
            .Where(p => p.UserId == userId)
            .ToListAsync();
        products.Should().NotBeEmpty();
        products.Should().AllSatisfy(p => p.IsDeleted.Should().BeTrue());
        
        var reactivateResponse = await adminUserClient.PutAsync($"/api/users/activate/{userId}", null);
        reactivateResponse.EnsureSuccessStatusCode();
        
        user = await userContext.user.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);
        user.Should().NotBeNull();
        user.IsActive.Should().BeTrue();
        
        products = await productContext.Products.AsNoTracking().IgnoreQueryFilters()
            .Where(p => p.UserId == userId)
            .ToListAsync();
        products.Should().NotBeEmpty();
        products.Should().AllSatisfy(p => p.IsDeleted.Should().BeFalse());
    }
    
    [Fact]
    public async Task DeactivateUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        /*Test data*/
        var adminRegisterRequest = new
        {
            Name = "Admin User",
            Email = "admin@example.com",
            Password = "AdminPassword123",
            Role = "Admin"
        };

        var adminToken = await RegisterAndLoginAsync(adminRegisterRequest);
        var adminUserClient = CreateAuthorizedClient(adminToken, UserClient.BaseAddress);
        var nonExistentUserId = 99999;
        
        var response = await adminUserClient.PutAsync($"/api/users/deactivate/{nonExistentUserId}", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("User with ID 99999 not found.");
    }
    
    [Fact]
    public async Task ActivateUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        /*Test data*/
        var adminRegisterRequest = new
        {
            Name = "Admin User",
            Email = "admin@example.com",
            Password = "AdminPassword123",
            Role = "Admin"
        };

        var adminToken = await RegisterAndLoginAsync(adminRegisterRequest);
        var adminUserClient = CreateAuthorizedClient(adminToken, UserClient.BaseAddress);
        var nonExistentUserId = 99999;
        
        var response = await adminUserClient.PutAsync($"/api/users/activate/{nonExistentUserId}", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("User with ID 99999 not found.");
    }
    
    [Fact]
    public async Task DeactivateUser_ShouldReturnForbidden_WhenUserIsNotAdmin()
    {
        /*Test data*/
        var userRegisterRequest = new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };

        var userToken = await RegisterAndLoginAsync(userRegisterRequest);
        var userClient = CreateAuthorizedClient(userToken, UserClient.BaseAddress);
        var userId = 1; 
        
        var response = await userClient.PutAsync($"/api/users/deactivate/{userId}", null);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
   
    [Fact]
    public async Task ActivateUser_ShouldReturnForbidden_WhenUserIsNotAdmin()
    {
        /*Test data*/
        var userRegisterRequest = new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };

        var userToken = await RegisterAndLoginAsync(userRegisterRequest);
        var userClient = CreateAuthorizedClient(userToken, UserClient.BaseAddress);
        var userId = 1; 
        
        var response = await userClient.PutAsync($"/api/users/activate/{userId}", null);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task DeactivateUser_ShouldSoftDeleteProducts_ButNotRemoveThemPermanently()
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

        /*Test data*/
        var productCreateRequest = new
        {
            Name = "Test Product",
            Description = "A test product",
            Price = 100.0,
            IsAvailable = true
        };

        var adminToken = await RegisterAndLoginAsync(adminRegisterRequest);
        var userToken = await RegisterAndLoginAsync(userRegisterRequest);

        var userProductClient = CreateAuthorizedClient(userToken, ProductClient.BaseAddress);
        var productResponse = await userProductClient.PostAsJsonAsync("/api/products", productCreateRequest);
        productResponse.EnsureSuccessStatusCode();

        var userId = await GetUserIdFromDatabaseAsync(userRegisterRequest.Email);
        var adminUserClient = CreateAuthorizedClient(adminToken, UserClient.BaseAddress);
        
        var deactivateResponse = await adminUserClient.PutAsync($"/api/users/deactivate/{userId}", null);
        deactivateResponse.EnsureSuccessStatusCode();
        
        await using var productContext = new ProductDbContext(ProductDbOptions);
        var products = await productContext.Products.IgnoreQueryFilters()
            .Where(p => p.UserId == userId)
            .ToListAsync();

        products.Should().NotBeEmpty();
        products.Should().AllSatisfy(p => p.IsDeleted.Should().BeTrue());
    }
    
    [Fact]
    public async Task ActivateUser_ShouldMakeProductsVisible_WhenUserIsReactivated()
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

        /*Test data*/
        var productCreateRequest = new
        {
            Name = "Test Product",
            Description = "A test product",
            Price = 100.0,
            IsAvailable = true
        };

        var adminToken = await RegisterAndLoginAsync(adminRegisterRequest);
        var userToken = await RegisterAndLoginAsync(userRegisterRequest);

        var userProductClient = CreateAuthorizedClient(userToken, ProductClient.BaseAddress);
        var productResponse = await userProductClient.PostAsJsonAsync("/api/products", productCreateRequest);
        productResponse.EnsureSuccessStatusCode();

        var userId = await GetUserIdFromDatabaseAsync(userRegisterRequest.Email);
        var adminUserClient = CreateAuthorizedClient(adminToken, UserClient.BaseAddress);
        
        var deactivateResponse = await adminUserClient.PutAsync($"/api/users/deactivate/{userId}", null);
        deactivateResponse.EnsureSuccessStatusCode();
        
        var reactivateResponse = await adminUserClient.PutAsync($"/api/users/activate/{userId}", null);
        reactivateResponse.EnsureSuccessStatusCode();
        
        await using var productContext = new ProductDbContext(ProductDbOptions);
        var products = await productContext.Products.AsNoTracking()
            .Where(p => p.UserId == userId)
            .ToListAsync();

        products.Should().NotBeEmpty();
        products.Should().AllSatisfy(p => p.IsDeleted.Should().BeFalse());
    }
    
    private HttpClient CreateAuthorizedClient(string token, Uri baseAddress)
    {
        var client = new HttpClient { BaseAddress = baseAddress };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}