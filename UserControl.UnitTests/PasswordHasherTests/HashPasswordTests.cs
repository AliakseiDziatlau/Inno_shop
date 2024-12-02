using FluentAssertions;

namespace UserControl.UnitTests.PasswordHasherTests;

public class HashPasswordTests : PasswordHasherTests
{
    [Fact]
    public void HashPassword_ShouldGenerateHashedPassword()
    {
        var password = "Test@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        hashedPassword.Should().NotBeNullOrWhiteSpace();
        hashedPassword.Should().NotBe(password); 
    }
    
    [Fact]
    public void HashPassword_ShouldGenerateDifferentHashesForSamePassword()
    {
        var password = "Test@123";
        var hashedPassword1 = _passwordHasher.HashPassword(password);
        var hashedPassword2 = _passwordHasher.HashPassword(password);
        hashedPassword1.Should().NotBe(hashedPassword2);
    }
    
    [Fact]
    public void HashPassword_ShouldGenerateHashForEmptyPassword()
    {
        var password = string.Empty;
        var hashedPassword = _passwordHasher.HashPassword(password);
        hashedPassword.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public void HashPassword_ShouldThrowExceptionForNullPassword()
    {
        string? password = null;
        Action action = () => _passwordHasher.HashPassword(password!);
        action.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void HashPassword_ShouldGenerateHashOfExpectedLength()
    {
        var password = "Test@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        hashedPassword.Length.Should().BeGreaterOrEqualTo(60);
    }
    
    [Fact]
    public void HashPassword_ShouldTakeReasonableTime()
    {
        var password = "Test@123";
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _passwordHasher.HashPassword(password);
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500); 
    }
}