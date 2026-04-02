using FluentAssertions;
using RedGreenBlue.Services;

namespace RedGreenBlue.UnitTests.Services;

public class CustomPasswordHasherTests
{
    [Fact]
    public void GenerateSalt_ShouldReturnBase64Encoded16ByteValue()
    {
        var sut = new CustomPasswordHasher();

        var salt = sut.GenerateSalt();

        var decoded = Convert.FromBase64String(salt);
        decoded.Should().HaveCount(16);
    }

    [Fact]
    public void GenerateSalt_ShouldReturnDifferentValuesAcrossCalls()
    {
        var sut = new CustomPasswordHasher();

        var first = sut.GenerateSalt();
        var second = sut.GenerateSalt();

        first.Should().NotBe(second);
    }

    [Fact]
    public void HashPassword_ThenVerify_ShouldSucceedForCorrectPassword()
    {
        var sut = new CustomPasswordHasher();
        var salt = sut.GenerateSalt();

        var hash = sut.HashPassword("pw12345", salt);

        hash.Should().NotBeNullOrWhiteSpace();
        sut.VerifyPassword(hash, "pw12345").Should().BeTrue();
        sut.VerifyPassword(hash, "wrongPassword").Should().BeFalse();
    }

    [Fact]
    public void HashPassword_ShouldThrow_WhenSaltIsInvalidBase64()
    {
        var sut = new CustomPasswordHasher();

        var act = () => sut.HashPassword("pw12345", "%%%invalid-base64%%%" );

        act.Should().Throw<FormatException>();
    }
}
