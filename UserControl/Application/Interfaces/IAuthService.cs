using UserControl.Application.DTOs;

namespace UserControl.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task RegisterAsync(RegisterRequestDto requestDto);
    Task RequestPasswordResetAsync(PasswordResetRequestDto requestDto);
    Task ResetPasswordAsync(string token, ResetPasswordRequestDto requestDto);
    Task ConfirmEmailAsync(string token);
}