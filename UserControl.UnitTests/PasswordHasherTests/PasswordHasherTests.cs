using UserControl.Infrastructure.Interfaces;
using UserControl.Infrastructure.Services;

namespace UserControl.UnitTests.PasswordHasherTests;

public abstract class PasswordHasherTests
{
    protected readonly IPasswordHasher _passwordHasher = new PasswordHasher(); 
}