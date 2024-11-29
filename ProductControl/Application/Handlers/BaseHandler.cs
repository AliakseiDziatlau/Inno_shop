using ProductControl.Infrastracture.Interfaces;
using ProductControl.Infrastracture.Services;

namespace ProductControl.Application.Handlers;

public class BaseHandler
{
    public ITokenService tokenService = new TokenService();
}