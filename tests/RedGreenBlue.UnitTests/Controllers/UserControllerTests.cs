using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RedGreenBlue.Controllers;
using RedGreenBlue.Dtos;
using RedGreenBlue.Dtos.User;
using RedGreenBlue.Models;
using RedGreenBlue.Services;
using RedGreenBlue.Services.Interfaces;

namespace RedGreenBlue.UnitTests.Controllers;

public class UserControllerTests
{
    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenTeamIsInvalid()
    {
        var authService = new Mock<IAuthService>();
        var jwtService = new Mock<IJwtService>();
        var controller = new UserController(authService.Object, jwtService.Object);

        var dto = new RegisterUserDto
        {
            Username = "NewUser",
            Password = "pw12345",
            Team = (TeamColor)99
        };

        var result = await controller.Register(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        authService.Verify(a => a.RegisterAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenUsernameAlreadyExists()
    {
        var authService = new Mock<IAuthService>();
        var jwtService = new Mock<IJwtService>();
        authService
            .Setup(a => a.RegisterAsync(It.IsAny<User>()))
            .ReturnsAsync((User?)null);

        var controller = new UserController(authService.Object, jwtService.Object);

        var dto = new RegisterUserDto
        {
            Username = "ExistingUser",
            Password = "pw12345",
            Team = TeamColor.Green
        };

        var result = await controller.Register(dto);

        var objectResult = result.Result.Should().BeOfType<ConflictObjectResult>().Subject;
        objectResult.Value.Should().Be("Username already exists");
    }

    [Fact]
    public async Task Register_ShouldReturn201AndUserDto_WhenRegistrationSucceeds()
    {
        var authService = new Mock<IAuthService>();
        var jwtService = new Mock<IJwtService>();

        authService
            .Setup(a => a.RegisterAsync(It.IsAny<User>()))
            .ReturnsAsync(new User
            {
                Id = 9,
                Username = "CreatedUser",
                Team = TeamColor.Blue,
                IsAdmin = false,
                Password = "hash",
                Salt = "salt"
            });

        var controller = new UserController(authService.Object, jwtService.Object);

        var dto = new RegisterUserDto
        {
            Username = "CreatedUser",
            Password = "pw12345",
            Team = TeamColor.Blue
        };

        var result = await controller.Register(dto);

        var created = result.Result.Should().BeOfType<ObjectResult>().Subject;
        created.StatusCode.Should().Be(201);

        var responseDto = created.Value.Should().BeOfType<UserResponseDto>().Subject;
        responseDto.Id.Should().Be(9);
        responseDto.Username.Should().Be("CreatedUser");
        responseDto.Team.Should().Be(TeamColor.Blue);
        responseDto.IsAdmin.Should().BeFalse();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        var authService = new Mock<IAuthService>();
        var jwtService = new Mock<IJwtService>();

        authService
            .Setup(a => a.LoginAsync("Nope", "wrong"))
            .ReturnsAsync((User?)null);

        var controller = new UserController(authService.Object, jwtService.Object);

        var result = await controller.Login(new LoginUserDto
        {
            Username = "Nope",
            Password = "wrong"
        });

        var unauthorized = result.Result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorized.Value.Should().Be("Invalid username or password");
    }

    [Fact]
    public async Task Login_ShouldReturnTokenAndUserData_WhenCredentialsAreValid()
    {
        var authService = new Mock<IAuthService>();
        var jwtService = new Mock<IJwtService>();

        var user = new User
        {
            Id = 1,
            Username = "Alice",
            Team = TeamColor.Red,
            IsAdmin = true,
            Password = "hash",
            Salt = "salt"
        };

        authService
            .Setup(a => a.LoginAsync("Alice", "pw12345"))
            .ReturnsAsync(user);
        jwtService
            .Setup(j => j.GenerateToken(user))
            .Returns("jwt-token");

        var controller = new UserController(authService.Object, jwtService.Object);

        var result = await controller.Login(new LoginUserDto
        {
            Username = "Alice",
            Password = "pw12345"
        });

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = ok.Value.Should().BeOfType<LoginResponseDto>().Subject;

        dto.Token.Should().Be("jwt-token");
        dto.Username.Should().Be("Alice");
        dto.Team.Should().Be(TeamColor.Red);
        dto.IsAdmin.Should().BeTrue();
    }
}
