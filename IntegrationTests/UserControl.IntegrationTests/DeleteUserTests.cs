using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductControl.Infrastracture.Persistence;
using UserControl.Infrastructure.Persistence;

namespace IntegrationTests.UserControl.IntegrationTests;

public class DeleteUserTests : IntegrationTestBase
{
    [Fact]
    public async Task DeleteUser_ShouldDeleteUserAndTheirProducts()
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
        var adminRegisterRequest = new
        {
            Name = "Admin User",
            Email = "admin@example.com",
            Password = "AdminPassword123",
            Role = "Admin"
        };
    
        /*Test data*/
        var productCreateRequest = new
        {
            Name = "Test Product",
            Description = "A test product",
            Price = 150.0,
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
    
        var productResponse = await userProductClient.PostAsJsonAsync("/api/products", productCreateRequest);
        productResponse.EnsureSuccessStatusCode();
        
        await using var productContext = new ProductDbContext(ProductDbOptions);
        var createdProduct = await productContext.Products
            .SingleOrDefaultAsync(p => p.Name == productCreateRequest.Name && p.Description == productCreateRequest.Description);
    
        createdProduct.Should().NotBeNull();
        createdProduct.UserId.Should().Be(await GetUserIdFromDatabaseAsync(userRegisterRequest.Email));
        
        var adminRegisterResponse = await UserClient.PostAsJsonAsync("/api/auths/register", adminRegisterRequest);
        adminRegisterResponse.EnsureSuccessStatusCode();
    
        var adminConfirmResponse = await UserClient.GetAsync($"/api/auths/confirm-email?token={await GetConfirmationTokenAsync(adminRegisterRequest.Email)}");
        adminConfirmResponse.EnsureSuccessStatusCode();
        
        var adminLoginResponse = await UserClient.PostAsJsonAsync("/api/auths/login", new
        {
            Email = adminRegisterRequest.Email,
            Password = adminRegisterRequest.Password
        });
        adminLoginResponse.EnsureSuccessStatusCode();
    
        var adminToken = await ExtractJwtToken(adminLoginResponse);
    
        var adminUserClient = new HttpClient { BaseAddress = new Uri("http://localhost:5001") };
        adminUserClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        
        var userId = await GetUserIdFromDatabaseAsync(userRegisterRequest.Email);
        var deleteUserResponse = await adminUserClient.DeleteAsync($"/api/users/{userId}");
        deleteUserResponse.EnsureSuccessStatusCode();
        
        await using var userContext = new UserDbContext(UserDbOptions);
        var deletedUser = await userContext.user.SingleOrDefaultAsync(u => u.Id == userId);
        deletedUser.Should().BeNull();
        
        var userProducts = await productContext.Products.Where(p => p.UserId == userId).ToListAsync();
        userProducts.Should().BeEmpty("all products associated with the deleted user should also be removed from the database.");
    }
    
    [Fact]
    public async Task DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
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

        var adminUserClient = new HttpClient { BaseAddress = new Uri("http://localhost:5001") };
        adminUserClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var nonExistentUserId = 99999;
       
        var deleteUserResponse = await adminUserClient.DeleteAsync($"/api/users/{nonExistentUserId}");
        deleteUserResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var responseContent = await deleteUserResponse.Content.ReadAsStringAsync();
        responseContent.Should().Contain("User with ID 99999 not found.");
    }
    
    [Fact]
    public async Task DeleteUser_ShouldReturnForbidden_WhenNonAdminAttemptsToDeleteUser()
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
        var userClient = new HttpClient { BaseAddress = new Uri("http://localhost:5001") };
        userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var userId = await GetUserIdFromDatabaseAsync(userRegisterRequest.Email);
        var deleteUserResponse = await userClient.DeleteAsync($"/api/users/{userId}");
        deleteUserResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}