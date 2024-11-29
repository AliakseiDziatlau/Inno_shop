using UserControl.Application.Interfaces;

namespace UserControl.Application.Handlers.AuthsControllerHandlers;

public class AuthHandlerBase
{
    protected readonly IAuthService _authService;

    protected AuthHandlerBase(IAuthService authService)
    {
        _authService = authService;
    }
}