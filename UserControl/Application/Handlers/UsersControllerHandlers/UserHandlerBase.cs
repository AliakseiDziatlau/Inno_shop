using UserControl.Application.Interfaces;

namespace UserControl.Application.Handlers.UsersControllerHandlers;

public class UserHandlerBase
{
    protected readonly IUserService _userService;

    protected UserHandlerBase(IUserService userService)
    {
        _userService = userService;
    }
}