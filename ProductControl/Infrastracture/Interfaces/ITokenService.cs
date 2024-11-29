using System.Security.Claims;

namespace ProductControl.Infrastracture.Interfaces;

public interface ITokenService
{
    int GetUserIdFromToken(ClaimsPrincipal user);
}