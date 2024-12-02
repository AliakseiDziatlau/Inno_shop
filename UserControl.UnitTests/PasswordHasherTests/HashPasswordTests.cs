using FluentAssertions;

namespace UserControl.UnitTests.PasswordHasherTests;

public class HashPasswordTests : PasswordHasherTests
{
    [Fact]
    public void HashPassword_ShouldGenerateHashedPassword()
    {
        var password = "Test@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        Assert.False(string.IsNullOrWhiteSpace(hashedPassword), "The hashed password should not be null or whitespace.");
        Assert.NotEqual(password, hashedPassword);
    }

    [Fact]
    public void HashPassword_ShouldGenerateDifferentHashesForSamePassword()
    {
        var password = "Test@123";
        var hashedPassword1 = _passwordHasher.HashPassword(password);
        var hashedPassword2 = _passwordHasher.HashPassword(password);
        Assert.NotEqual(hashedPassword1, hashedPassword2);
    }

    [Fact]
    public void HashPassword_ShouldGenerateHashForEmptyPassword()
    {
        var password = string.Empty;
        var hashedPassword = _passwordHasher.HashPassword(password);
        Assert.False(string.IsNullOrWhiteSpace(hashedPassword), "The hashed password for an empty string should not be null or whitespace.");
    }

    [Fact]
    public void HashPassword_ShouldThrowExceptionForNullPassword()
    {
        string? password = null;
        Assert.Throws<ArgumentNullException>(() => _passwordHasher.HashPassword(password!));
    }

    [Fact]
    public void HashPassword_ShouldGenerateHashOfExpectedLength()
    {
        var password = "Test@123";
        var hashedPassword = _passwordHasher.HashPassword(password);
        Assert.True(hashedPassword.Length >= 60, "The length of the hashed password should be at least 60 characters.");
    }

    [Fact]
    public void HashPassword_ShouldTakeReasonableTime()
    {
        var password = "Test@123";
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _passwordHasher.HashPassword(password);
        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds < 500, "Hashing the password should take less than 500 milliseconds.");
    }
}