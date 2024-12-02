using FluentAssertions;

namespace UserControl.UnitTests.PasswordHasherTests;

public class VerifyPasswordTests : global::UserControl.UnitTests.PasswordHasherTests.PasswordHasherTests
{
    [Fact]
    public void VerifyPassword_ShouldReturnTrueForValidPassword()
    {
        var password = "Test@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword(password, hashedPassword);
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalseForInvalidPassword()
    {
        var password = "Test@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword("WrongPassword", hashedPassword);
        result.Should().BeFalse();
    }
    
    [Fact]
    public void VerifyPassword_ShouldReturnFalseForEmptyPassword()
    {
        var password = "Test@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword(string.Empty, hashedPassword);
        result.Should().BeFalse();
    }
}