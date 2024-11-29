using System.Security.Claims;
using ProductControl.Infrastracture.Interfaces;

namespace ProductControl.Infrastracture.Services;

public class TokenService : ITokenService
{
    public int GetUserIdFromToken(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims
            .Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
            .FirstOrDefault(c => int.TryParse(c.Value, out _))
            ?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User identifier is missing or invalid in the token.");
        }

        return userId;
    }
}