using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using RedGreenBlue.Helpers;
using RedGreenBlue.Models;
using RedGreenBlue.Services;

namespace RedGreenBlue.UnitTests.Services;

public class JwtServiceTests
{
    [Fact]
    public void GenerateToken_ShouldContainExpectedClaimsAndMetadata()
    {
        var settings = new JwtSettings
        {
            Key = "unit-tests-very-secret-key-1234567890",
            Issuer = "https://issuer.test",
            Audience = "https://audience.test",
            ExpiresInMinutes = 30
        };

        var user = new User
        {
            Id = 42,
            Username = "TokenUser",
            Team = TeamColor.Blue,
            IsAdmin = true,
            Password = "unused",
            Salt = "unused"
        };

        var sut = new JwtService(Options.Create(settings));

        var before = DateTime.UtcNow;
        var token = sut.GenerateToken(user);
        var after = DateTime.UtcNow;

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Issuer.Should().Be(settings.Issuer);
        jwt.Audiences.Should().Contain(settings.Audience);
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == user.Username)
            .And.Contain(c => c.Type == "team" && c.Value == TeamColor.Blue.ToString())
            .And.Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin")
            .And.Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());

        jwt.ValidTo.Should().BeOnOrAfter(before.AddMinutes(settings.ExpiresInMinutes).AddSeconds(-5));
        jwt.ValidTo.Should().BeOnOrBefore(after.AddMinutes(settings.ExpiresInMinutes).AddSeconds(5));
    }
}
