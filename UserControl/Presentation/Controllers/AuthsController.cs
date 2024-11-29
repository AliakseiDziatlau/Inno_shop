using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserControl.Application.Commands.AuthsControllerCommands;
namespace UserControl.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var loginResponse = await _mediator.Send(command);
        return Ok(loginResponse);
    }
    
    /*
     If user sends request to reset his password he will receive a confirmation message to his email
     */
    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetCommand command)
    {
        await _mediator.Send(command);
        return Ok("Password reset link sent to your email.");
    }
    
    /*
     When user receives a confirmation message he can set a new password using a token from confirmation message
     */
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] ResetPasswordCommand command)
    {
        command.Token = token; // Передаем токен в команду
        await _mediator.Send(command);
        return Ok("Password has been reset successfully.");
    }
    
    /*
     When user is registered an email with a confirmation is sent to user,
     user also is entered into DB but is marked as not confirmed  
     */
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        await _mediator.Send(command);
        return Ok("Registration successful! Please check your email to confirm your account.");
    }
    
    /*
     A confirmation link is sent to users email, if user follows this link, he becomes confirmed
     and marked as confirmed in the DB
     */
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
        var command = new ConfirmEmailCommand(token);
        await _mediator.Send(command);
        return Ok("Your account has been confirmed successfully.");
    }
}