using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RedGreenBlue.Dtos;
using RedGreenBlue.Dtos.User;
using RedGreenBlue.IntegrationTests.Infrastructure;
using RedGreenBlue.Models;

namespace RedGreenBlue.IntegrationTests.Endpoints;

public class UserEndpointsTests : ApiTestBase
{
    public UserEndpointsTests(TestWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Register_ShouldReturnCreatedAndUserDto_WhenRequestIsValid()
    {
        var response = await Client.PostAsJsonAsync("/api/user/register", new RegisterUserDto
        {
            Username = "ApiUser1",
            Password = "pw12345",
            Team = TeamColor.Green
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var payload = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        payload.Should().NotBeNull();

        if (payload is null)
        {
            throw new InvalidOperationException("Register response payload was null.");
        }

        payload.Username.Should().Be("ApiUser1");
        payload.Team.Should().Be(TeamColor.Green);
        payload.IsAdmin.Should().BeFalse();
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenUsernameAlreadyExists()
    {
        var request = new RegisterUserDto
        {
            Username = "DuplicateUser",
            Password = "pw12345",
            Team = TeamColor.Red
        };

        var first = await Client.PostAsJsonAsync("/api/user/register", request);
        var second = await Client.PostAsJsonAsync("/api/user/register", request);

        first.StatusCode.Should().Be(HttpStatusCode.Created);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsWrong()
    {
        await Client.PostAsJsonAsync("/api/user/register", new RegisterUserDto
        {
            Username = "LoginUser",
            Password = "pw12345",
            Team = TeamColor.Blue
        });

        var response = await Client.PostAsJsonAsync("/api/user/login", new LoginUserDto
        {
            Username = "LoginUser",
            Password = "wrong"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ShouldReturnTokenAndClaimsData_WhenCredentialsAreValid()
    {
        await Client.PostAsJsonAsync("/api/user/register", new RegisterUserDto
        {
            Username = "LoginUser2",
            Password = "pw12345",
            Team = TeamColor.Red
        });

        var response = await Client.PostAsJsonAsync("/api/user/login", new LoginUserDto
        {
            Username = "LoginUser2",
            Password = "pw12345"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        payload.Should().NotBeNull();

        if (payload is null)
        {
            throw new InvalidOperationException("Login response payload was null.");
        }

        payload.Token.Should().NotBeNullOrWhiteSpace();
        payload.Username.Should().Be("LoginUser2");
        payload.Team.Should().Be(TeamColor.Red);
    }
}
