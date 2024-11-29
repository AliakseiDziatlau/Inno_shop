using UserControl.Application.DTOs;
using UserControl.Application.Interfaces;
using UserControl.Core.Entities;
using UserControl.Core.Interfaces;
using UserControl.Infrastructure.Interfaces;
namespace UserControl.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IEmailService emailService,IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid login credentials.");
        }

        if (!user.IsConfirmed)
        {
            throw new UnauthorizedAccessException("Account not confirmed. Please check your email.");
        }
        
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Your account has been deactivated. Please contact support.");
        }

        var token = _jwtTokenGenerator.GenerateJwtToken(user);
        return new LoginResponseDto { Token = token };
    }

    public async Task RegisterAsync(RegisterRequestDto requestDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(requestDto.Email);
        if (existingUser != null)
        {
            throw new ArgumentException("A user with this email already exists.");
        }

        var user = new User
        {
            Name = requestDto.Name,
            Email = requestDto.Email,
            Role = requestDto.Role,
            PasswordHash = _passwordHasher.HashPassword(requestDto.Password),
            IsActive = true,
            IsConfirmed = false,
            ConfirmationToken = Guid.NewGuid().ToString()
        };

        await _userRepository.AddAsync(user);

        /*Link to be sent to the users email*/
        var pathToUserServiceForConfirmation = _configuration["PathToUserServiceForConfirmation"];
        var confirmLink = $"{pathToUserServiceForConfirmation}/api/auths/confirm-email?token={user.ConfirmationToken}";
        var subject = "Confirm your account";
        var body = $"<p>To confirm your account, please click the link below:</p><p><a href='{confirmLink}'>Confirm Account</a></p>";

        await _emailService.SendEmailAsync(user.Email, subject, body);
    }

    public async Task RequestPasswordResetAsync(PasswordResetRequestDto requestDto)
    {
        var user = await _userRepository.GetByEmailAsync(requestDto.Email);
        if (user == null)
        {
            throw new KeyNotFoundException("User with the specified email was not found.");
        }
        
        user.PasswordResetToken = Guid.NewGuid().ToString();
        user.PasswordResetTokenExpiryTime = DateTime.UtcNow.AddHours(1);

        await _userRepository.UpdateAsync(user);

        /*Link to be sent to the users email*/
        var pathToUserServiceForConfirmation = _configuration["PathToUserServiceForConfirmation"];
        var resetLink = $"{pathToUserServiceForConfirmation}/api/auths/reset-password?token={user.PasswordResetToken}";
        var subject = "Password Reset Request";
        var body = $"<p>To reset your password, please click the link below:</p><p><a href='{resetLink}'>Reset Password</a></p>";

        await _emailService.SendEmailAsync(user.Email, subject, body);
    }

    public async Task ResetPasswordAsync(string token, ResetPasswordRequestDto requestDto)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(token);

        if (user == null || user.PasswordResetTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired password reset token.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(requestDto.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiryTime = null;

        await _userRepository.UpdateAsync(user);
    }

    public async Task ConfirmEmailAsync(string token)
    {
        var user = await _userRepository.GetByConfirmationTokenAsync(token);

        if (user == null)
        {
            throw new KeyNotFoundException("Invalid confirmation token.");
        }

        user.IsConfirmed = true;
        user.ConfirmationToken = null;

        await _userRepository.UpdateAsync(user);
    }
}