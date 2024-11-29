using ProductControl.Infrastracture.Interfaces;
using ProductControl.Infrastracture.Services;

namespace ProductControl.Application.Handlers;

public class ProductHandlerBase
{
    public ITokenService tokenService = new TokenService();
}