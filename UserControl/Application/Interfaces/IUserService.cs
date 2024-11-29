using UserControl.Application.DTOs;

namespace UserControl.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task UpdateUserAsync(int id, UpdateUserRequestDto requestDto);
    Task DeleteUserAsync(int id);
    Task ActivateUserAsync(int id);
    Task DeactivateUserAsync(int id);
}