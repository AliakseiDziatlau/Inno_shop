using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductControl.Infrastracture.Persistence;
using UserControl.Infrastructure.Persistence;
namespace IntegrationTests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient UserClient;
    protected readonly HttpClient ProductClient;
    protected readonly DbContextOptions<UserDbContext> UserDbOptions;
    protected readonly DbContextOptions<ProductDbContext> ProductDbOptions;

    public HttpClient CreateNewUserClient() => new HttpClient { BaseAddress = new Uri("http://localhost:5001") };
    public HttpClient CreateNewProductClient() => new HttpClient { BaseAddress = new Uri("http://localhost:5002") };

    protected IntegrationTestBase()
    {
        UserClient = new HttpClient { BaseAddress = new Uri("http://localhost:5001") };
        ProductClient = new HttpClient { BaseAddress = new Uri("http://localhost:5002") };
        
        UserDbOptions = new DbContextOptionsBuilder<UserDbContext>()
            .UseNpgsql("Host=localhost;Port=5435;Database=userdbtest;Username=postgres;Password=Alexey200616may")
            .Options;

        ProductDbOptions = new DbContextOptionsBuilder<ProductDbContext>()
            .UseNpgsql("Host=localhost;Port=5436;Database=productdbtest;Username=postgres;Password=Alexey200616may")
            .Options;
    }

    public async Task InitializeAsync()
    {
        using var userContext = new UserDbContext(UserDbOptions);
        using var productContext = new ProductDbContext(ProductDbOptions);

        await TruncateUserDatabaseAsync(userContext);
        await TruncateProductDatabaseAsync(productContext);
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    protected async Task<string> GetConfirmationTokenAsync(string email)
    {
        await using var userContext = new UserDbContext(UserDbOptions);
        var user = await userContext.user.SingleOrDefaultAsync(u => u.Email == email);
        return user?.ConfirmationToken ?? throw new InvalidOperationException("Confirmation token not found.");
    }

    protected async Task<int> GetUserIdFromDatabaseAsync(string email)
    {
        await using var userContext = new UserDbContext(UserDbOptions);
        var user = await userContext.user.SingleOrDefaultAsync(u => u.Email == email);
        return user?.Id ?? throw new InvalidOperationException("User not found in the database.");
    }

    protected async Task<string> ExtractJwtToken(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseJson = JsonDocument.Parse(responseContent).RootElement;
        responseJson.TryGetProperty("token", out var jwtToken).Should().BeTrue();
        return jwtToken.GetString();
    }

    private async Task TruncateUserDatabaseAsync(DbContext context)
    {
        var table = "user";
        await context.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE \"{table}\" RESTART IDENTITY CASCADE;");
    }

    private async Task TruncateProductDatabaseAsync(DbContext context)
    {
        var table = "Products";
        await context.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE \"{table}\" RESTART IDENTITY CASCADE;");
    }
    
    protected async Task ConfirmEmailAsync(string email)
    {
        var confirmationToken = await GetConfirmationTokenAsync(email);
        var confirmResponse = await UserClient.GetAsync($"/api/auths/confirm-email?token={confirmationToken}");
        confirmResponse.EnsureSuccessStatusCode();
    }
    
    protected async Task<string> RegisterAndLoginAsync(object registerRequest)
    {
        var registerResponse = await UserClient.PostAsJsonAsync("/api/auths/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();
        
        var email = registerRequest.GetType().GetProperty("Email")?.GetValue(registerRequest)?.ToString();
        if (string.IsNullOrEmpty(email))
        {
            throw new InvalidOperationException("Email is missing in the register request.");
        }
        await ConfirmEmailAsync(email);
        var loginRequest = new
        {
            Email = email,
            Password = registerRequest.GetType().GetProperty("Password")?.GetValue(registerRequest)?.ToString()
        };

        var loginResponse = await UserClient.PostAsJsonAsync("/api/auths/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        
        return await ExtractJwtToken(loginResponse);
    }
}
