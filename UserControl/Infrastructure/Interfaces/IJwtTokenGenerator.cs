using UserControl.Core.Entities;

namespace UserControl.Infrastructure.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(User user);
}