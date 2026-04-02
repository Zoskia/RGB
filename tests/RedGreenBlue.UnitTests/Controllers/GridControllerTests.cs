using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RedGreenBlue.Controllers;
using RedGreenBlue.Dtos;
using RedGreenBlue.Models;
using RedGreenBlue.Services.Interfaces;

namespace RedGreenBlue.UnitTests.Controllers;

public class GridControllerTests
{
    [Fact]
    public async Task GetGrid_ShouldReturnBadRequest_WhenTeamColorIsInvalid()
    {
        var service = new Mock<IGridService>();
        var controller = CreateController(service.Object);

        var result = await controller.GetGrid((TeamColor)99);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        service.Verify(s => s.GetCellsAsync(It.IsAny<TeamColor>()), Times.Never);
    }

    [Fact]
    public async Task GetGrid_ShouldReturnCells_WhenTeamColorIsValid()
    {
        var cells = new List<Cell>
        {
            new() { Q = 1, R = 1, TeamColor = TeamColor.Red, HexColor = "#ff0000" }
        };

        var service = new Mock<IGridService>();
        service.Setup(s => s.GetCellsAsync(TeamColor.Red)).ReturnsAsync(cells);

        var controller = CreateController(service.Object);

        var result = await controller.GetGrid(TeamColor.Red);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(cells);
    }

    [Fact]
    public async Task UpdateCellColor_ShouldReturnBadRequest_WhenTeamColorIsInvalid()
    {
        var service = new Mock<IGridService>();
        var controller = CreateController(service.Object, CreateUser(isAdmin: true));

        var result = await controller.UpdateCellColorAsync(new UpdateCellColorDto
        {
            Q = 0,
            R = 0,
            TeamColor = (TeamColor)99,
            HexColor = "#ff0000"
        });

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Invalid team color");
    }

    [Fact]
    public async Task UpdateCellColor_ShouldReturnForbidden_WhenTeamClaimIsMissingForNonAdmin()
    {
        var service = new Mock<IGridService>();
        var controller = CreateController(service.Object, CreateUser(isAdmin: false));

        var result = await controller.UpdateCellColorAsync(new UpdateCellColorDto
        {
            Q = 1,
            R = 1,
            TeamColor = TeamColor.Red,
            HexColor = "#ff0000"
        });

        var forbidden = result.Should().BeOfType<ObjectResult>().Subject;
        forbidden.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        forbidden.Value.Should().Be("User team claim is missing or invalid.");
    }

    [Fact]
    public async Task UpdateCellColor_ShouldReturnForbidden_WhenUserTriesToEditAnotherTeam()
    {
        var service = new Mock<IGridService>();
        var controller = CreateController(service.Object, CreateUser(isAdmin: false, teamClaim: TeamColor.Red.ToString()));

        var result = await controller.UpdateCellColorAsync(new UpdateCellColorDto
        {
            Q = 1,
            R = 1,
            TeamColor = TeamColor.Blue,
            HexColor = "#0000ff"
        });

        var forbidden = result.Should().BeOfType<ObjectResult>().Subject;
        forbidden.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        forbidden.Value.Should().Be("You can only update cells for your own team.");
        service.Verify(s => s.UpdateCellColorAsync(It.IsAny<UpdateCellColorDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCellColor_ShouldAcceptNumericTeamClaim_WhenNonAdmin()
    {
        var service = new Mock<IGridService>();
        service.Setup(s => s.UpdateCellColorAsync(It.IsAny<UpdateCellColorDto>())).ReturnsAsync(true);

        var controller = CreateController(service.Object, CreateUser(isAdmin: false, teamClaim: "1"));

        var result = await controller.UpdateCellColorAsync(new UpdateCellColorDto
        {
            Q = 4,
            R = 8,
            TeamColor = TeamColor.Green,
            HexColor = "#00ff00"
        });

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateCellColor_ShouldAllowAdminToUpdateAnyTeam()
    {
        var service = new Mock<IGridService>();
        service.Setup(s => s.UpdateCellColorAsync(It.IsAny<UpdateCellColorDto>())).ReturnsAsync(true);

        var controller = CreateController(service.Object, CreateUser(isAdmin: true));

        var result = await controller.UpdateCellColorAsync(new UpdateCellColorDto
        {
            Q = 2,
            R = 3,
            TeamColor = TeamColor.Blue,
            HexColor = "#0000ff"
        });

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateCellColor_ShouldReturnNotFound_WhenServiceReturnsFalse()
    {
        var service = new Mock<IGridService>();
        service.Setup(s => s.UpdateCellColorAsync(It.IsAny<UpdateCellColorDto>())).ReturnsAsync(false);

        var controller = CreateController(service.Object, CreateUser(isAdmin: true));

        var result = await controller.UpdateCellColorAsync(new UpdateCellColorDto
        {
            Q = 10,
            R = 10,
            TeamColor = TeamColor.Red,
            HexColor = "#ff0000"
        });

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateCellColor_ShouldReturnBadRequest_WhenSpectrumValidationFailsInService()
    {
        var service = new Mock<IGridService>();
        service
            .Setup(s => s.UpdateCellColorAsync(It.IsAny<UpdateCellColorDto>()))
            .ThrowsAsync(new InvalidOperationException("Wrong color spectrum"));

        var controller = CreateController(service.Object, CreateUser(isAdmin: true));

        var result = await controller.UpdateCellColorAsync(new UpdateCellColorDto
        {
            Q = 1,
            R = 1,
            TeamColor = TeamColor.Red,
            HexColor = "#0000ff"
        });

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Wrong color spectrum");
    }

    [Theory]
    [InlineData(typeof(FormatException))]
    [InlineData(typeof(ArgumentOutOfRangeException))]
    public async Task UpdateCellColor_ShouldReturnBadRequest_WhenServiceThrowsFormatRelatedException(Type exceptionType)
    {
        var service = new Mock<IGridService>();
        var exception = (Exception)Activator.CreateInstance(exceptionType, "invalid")!;

        service
            .Setup(s => s.UpdateCellColorAsync(It.IsAny<UpdateCellColorDto>()))
            .ThrowsAsync(exception);

        var controller = CreateController(service.Object, CreateUser(isAdmin: true));

        var result = await controller.UpdateCellColorAsync(new UpdateCellColorDto
        {
            Q = 1,
            R = 1,
            TeamColor = TeamColor.Green,
            HexColor = "#00ff00"
        });

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Invalid HEX color format.");
    }

    private static GridController CreateController(IGridService service, ClaimsPrincipal? user = null)
    {
        var controller = new GridController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user ?? new ClaimsPrincipal(new ClaimsIdentity())
                }
            }
        };

        return controller;
    }

    private static ClaimsPrincipal CreateUser(bool isAdmin, string? teamClaim = null)
    {
        var claims = new List<Claim>();

        if (isAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, "User"));
        }

        if (!string.IsNullOrWhiteSpace(teamClaim))
        {
            claims.Add(new Claim("team", teamClaim));
        }

        var identity = new ClaimsIdentity(claims, authenticationType: "TestAuth");
        return new ClaimsPrincipal(identity);
    }
}
