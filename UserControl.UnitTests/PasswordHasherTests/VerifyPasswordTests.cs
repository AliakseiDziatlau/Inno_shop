using FluentAssertions;

namespace UserControl.UnitTests.PasswordHasherTests;

public class VerifyPasswordTests : PasswordHasherTests
{
    [Fact]
    public void VerifyPassword_ShouldReturnTrueForValidPassword()
    {
        var password = "Test@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword(password, hashedPassword);
        Assert.True(result, "The password should be verified successfully.");
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalseForInvalidPassword()
    {
        var password = "Test@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword("WrongPassword", hashedPassword);
        Assert.False(result, "The password verification should fail for an invalid password.");
    }
    
    [Fact]
    public void VerifyPassword_ShouldReturnFalseForEmptyPassword()
    {
        var password = "Test@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword(string.Empty, hashedPassword);
        Assert.False(result, "The password verification should fail for an empty password.");
    }
}