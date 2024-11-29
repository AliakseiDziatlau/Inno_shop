using System.Net.Http.Headers;
using AutoMapper;
using UserControl.Application.DTOs;
using UserControl.Application.Interfaces;
using UserControl.Core.Interfaces;
namespace UserControl.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _userRepository = userRepository;
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task UpdateUserAsync(int id, UpdateUserRequestDto requestDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
        
        _mapper.Map(requestDto, existingUser);

        await _userRepository.UpdateAsync(existingUser);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        await _userRepository.DeleteAsync(user);
        await NotifyProductServiceUserDeletedAsync(id);
    }

    public async Task ActivateUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        user.IsActive = true;
        await _userRepository.UpdateAsync(user);
        
        await NotifyProductServiceUserStatusChangedAsync(userId, true);
    }

    public async Task DeactivateUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        user.IsActive = false;
        await _userRepository.UpdateAsync(user);
        
        await NotifyProductServiceUserStatusChangedAsync(userId, false);
    }
    
    /*
     This method is called when user is deleted for deleting all products of this user
      (not Soft Delete)
     */
    private async Task NotifyProductServiceUserDeletedAsync(int userId)
    {
        var client = _httpClientFactory.CreateClient("ProductService");
        
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException("Authorization token is missing.");
        }
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
        
        var response = await client.DeleteAsync($"/api/products/user/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Failed to delete products for user {userId}. Status: {response.StatusCode}");
        }
    }
    
    /*
      This method marks users products as IsDeleted "true" when user is deactivated,
      marks users products s IsDeleted "false" when user is activated 
      (Soft Delete)
     */
    private async Task NotifyProductServiceUserStatusChangedAsync(int userId, bool isActive)
    {
        var client = _httpClientFactory.CreateClient("ProductService");
        
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException("Authorization token is missing.");
        }
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

        var response = await client.PutAsJsonAsync($"/api/products/toggle-user-products/{userId}", isActive);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Failed to update product status for user {userId}. Status: {response.StatusCode}");
        }
    }
}