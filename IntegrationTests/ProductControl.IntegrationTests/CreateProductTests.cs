using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductControl.Core.Entities;
using ProductControl.Infrastracture.Persistence;

namespace IntegrationTests.ProductControl.IntegrationTests;

public class CreateProductTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenRequestIsNotValid()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };

        /*Test data*/
        var invalidProductCreateRequest = new
        {
            Name = "", 
            Description = "Invalid test product",
            Price = -1, 
            IsAvailable = true
        };
        
        var userToken = await RegisterAndLoginAsync(registerRequest);
        ProductClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        
        var productResponse = await ProductClient.PostAsJsonAsync("/api/products", invalidProductCreateRequest);
        productResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseContent = await productResponse.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrWhiteSpace();

        var responseJson = JsonDocument.Parse(responseContent).RootElement;
        responseJson.TryGetProperty("errors", out var errors).Should().BeTrue();
        
        if (string.IsNullOrWhiteSpace(invalidProductCreateRequest.Name))
        {
            errors.TryGetProperty("Name", out var nameErrors).Should().BeTrue();
            nameErrors.ToString().Should().Contain("Product name is required.");
        }

        if (invalidProductCreateRequest.Price <= 0)
        {
            errors.TryGetProperty("Price", out var priceErrors).Should().BeTrue();
            priceErrors.ToString().Should().Contain("Price must be greater than zero.");
        }
    }

    [Fact]
    public async Task CreateProduct_ShouldSaveProductInDatabase_WhenRequestIsValid()
    {
        /*Test data*/
        var registerRequest = new
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
            Price = 99.99,
            IsAvailable = true
        };
        
        var userToken = await RegisterAndLoginAsync(registerRequest);
        ProductClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        
        var productResponse = await ProductClient.PostAsJsonAsync("/api/products", productCreateRequest);
        productResponse.EnsureSuccessStatusCode();
        
        var responseContent = await productResponse.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrWhiteSpace();

        var productResponseData = JsonDocument.Parse(responseContent).RootElement;
        productResponseData.TryGetProperty("id", out var productId).Should().BeTrue();
        productResponseData.GetProperty("name").GetString().Should().Be(productCreateRequest.Name);
        productResponseData.GetProperty("description").GetString().Should().Be(productCreateRequest.Description);
        productResponseData.GetProperty("price").GetDecimal().Should().Be((decimal)productCreateRequest.Price);
        productResponseData.GetProperty("isAvailable").GetBoolean().Should().Be(productCreateRequest.IsAvailable);
        
        await using var productContext = new ProductDbContext(ProductDbOptions);
        var savedProduct = await productContext.Products.SingleOrDefaultAsync(p => p.Id == productId.GetInt32());
        savedProduct.Should().NotBeNull();
        savedProduct.Name.Should().Be(productCreateRequest.Name);
        savedProduct.Description.Should().Be(productCreateRequest.Description);
        savedProduct.Price.Should().Be((decimal)productCreateRequest.Price);
        savedProduct.IsAvailable.Should().Be(productCreateRequest.IsAvailable);
    }

    [Fact]
    public async Task CreateProduct_AfterUserCreation_ShouldLinkProductWithUserId()
    {
        /*Test data*/
        var registerRequest = new
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
            Price = 150.0,
            IsAvailable = true
        };
        
        var userToken = await RegisterAndLoginAsync(registerRequest);
        ProductClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        
        var productResponse = await ProductClient.PostAsJsonAsync("/api/products", productCreateRequest);
        productResponse.EnsureSuccessStatusCode();
        
        await using var productContext = new ProductDbContext(ProductDbOptions);
        var createdProduct = await productContext.Products
            .SingleOrDefaultAsync(p => p.Name == productCreateRequest.Name && p.Description == productCreateRequest.Description);

        createdProduct.Should().NotBeNull();
        createdProduct.UserId.Should().Be(await GetUserIdFromDatabaseAsync(registerRequest.Email));
        createdProduct.Price.Should().Be((decimal)productCreateRequest.Price);
        createdProduct.IsAvailable.Should().Be(productCreateRequest.IsAvailable);
    }
    
    [Fact]
    public async Task CreateProduct_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        /*Test data*/
        var productCreateRequest = new
        {
            Name = "Unauthorized Product",
            Description = "A test product without authentication",
            Price = 50.0,
            IsAvailable = true
        };
        
        var productResponse = await ProductClient.PostAsJsonAsync("/api/products", productCreateRequest);
        productResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CreateProduct_ShouldAllowMinimumPrice_WhenValid()
    {
        /*Test data*/
        var registerRequest = new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        };

        /*Test data*/
        var productCreateRequest = new
        {
            Name = "Min Price Product",
            Description = "Product with the minimum price",
            Price = 0.01, 
            IsAvailable = true
        };

        var userToken = await RegisterAndLoginAsync(registerRequest);
        ProductClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        
        var productResponse = await ProductClient.PostAsJsonAsync("/api/products", productCreateRequest);
        productResponse.EnsureSuccessStatusCode();
        var responseContent = await productResponse.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Min Price Product");
    }
    
    [Fact]
    public async Task CreateProduct_ShouldReturnValidationError_WhenNameIsEmpty()
    {
        /*Test data*/
        var token = await RegisterAndLoginAsync(new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        });

        using var productClient = CreateNewProductClient();
        productClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        /*Test data*/
        var invalidRequest = new
        {
            Name = "",
            Description = "A valid description",
            Price = 10.0,
            IsAvailable = true
        };
        
        var response = await productClient.PostAsJsonAsync("/api/products", invalidRequest);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Product name is required.");
    }
    
    [Fact]
    public async Task CreateProduct_ShouldReturnValidationError_WhenDescriptionIsEmpty()
    {
        /*Test data*/
        var token = await RegisterAndLoginAsync(new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        });

        using var productClient = CreateNewProductClient();
        productClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        /*Test data*/
        var invalidRequest = new
        {
            Name = "Valid Name",
            Description = "",
            Price = 10.0,
            IsAvailable = true
        };
        
        var response = await productClient.PostAsJsonAsync("/api/products", invalidRequest);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Product description is required.");
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnValidationError_WhenPriceIsZero()
    {
        /*Test data*/
        var token = await RegisterAndLoginAsync(new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        });

        using var productClient = CreateNewProductClient();
        productClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        /*Test data*/
        var invalidRequest = new
        {
            Name = "Valid Name",
            Description = "Valid Description",
            Price = 0.0,
            IsAvailable = true
        };
        
        var response = await productClient.PostAsJsonAsync("/api/products", invalidRequest);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Price must be greater than zero.");
    }
    
    [Fact]
    public async Task CreateProduct_ShouldReturnValidationError_WhenIsAvailableIsNull()
    {
        /*Test data*/
        var token = await RegisterAndLoginAsync(new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        });

        using var productClient = CreateNewProductClient();
        productClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        /*Test data*/
        var invalidRequest = new
        {
            Name = "Valid Name",
            Description = "Valid Description",
            Price = 10.0,
            IsAvailable = (bool?)null
        };
        
        var response = await productClient.PostAsJsonAsync("/api/products", invalidRequest);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("The command field is required.");
    }
    
    [Fact]
    public async Task CreateProduct_ShouldCreateProduct_WhenRequestIsValid()
    {
        /*Test data*/
        var token = await RegisterAndLoginAsync(new
        {
            Name = "Test User",
            Email = "testuser@example.com",
            Password = "TestPassword123",
            Role = "User"
        });

        using var productClient = CreateNewProductClient();
        productClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        /*Test data*/
        var validRequest = new
        {
            Name = "Valid Name",
            Description = "Valid Description",
            Price = 10.0,
            IsAvailable = true
        };
        
        var response = await productClient.PostAsJsonAsync("/api/products", validRequest);
        
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<Product>();
        responseContent.Should().NotBeNull();
        responseContent.Name.Should().Be(validRequest.Name);
        responseContent.Description.Should().Be(validRequest.Description);
        responseContent.Price.Should().Be((decimal)validRequest.Price);
        responseContent.IsAvailable.Should().Be(validRequest.IsAvailable);
    }
}