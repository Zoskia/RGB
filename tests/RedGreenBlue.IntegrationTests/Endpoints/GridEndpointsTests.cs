using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RedGreenBlue.Dtos;
using RedGreenBlue.IntegrationTests.Infrastructure;
using RedGreenBlue.Models;

namespace RedGreenBlue.IntegrationTests.Endpoints;

public class GridEndpointsTests : ApiTestBase
{
    public GridEndpointsTests(TestWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetGrid_ShouldReturnUnauthorized_WhenNoTokenIsProvided()
    {
        var response = await Client.GetAsync("/api/grid/0");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetGrid_ShouldReturnCellsForRequestedTeam_WhenAuthenticated()
    {
        var token = await RegisterAndLoginAsync("RedGridUser", "pw12345", TeamColor.Red);

        await SeedCellsAsync(
            new Cell { Q = 0, R = 0, TeamColor = TeamColor.Red, HexColor = "#ff0000" },
            new Cell { Q = 1, R = 0, TeamColor = TeamColor.Red, HexColor = "#cc0000" },
            new Cell { Q = 0, R = 0, TeamColor = TeamColor.Blue, HexColor = "#0000ff" });

        SetBearerToken(Client, token);

        var response = await Client.GetAsync("/api/grid/0");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<List<Cell>>();
        payload.Should().NotBeNull();

        if (payload is null)
        {
            throw new InvalidOperationException("Grid payload was null.");
        }

        payload.Should().HaveCount(2);
        payload.Should().OnlyContain(c => c.TeamColor == TeamColor.Red);
    }

    [Fact]
    public async Task UpdateCellColor_ShouldReturnForbidden_WhenNonAdminUpdatesAnotherTeam()
    {
        var token = await RegisterAndLoginAsync("RedEditor", "pw12345", TeamColor.Red);
        await SeedCellsAsync(new Cell { Q = 3, R = 3, TeamColor = TeamColor.Blue, HexColor = "#0000aa" });

        SetBearerToken(Client, token);

        var response = await Client.PutAsJsonAsync("/api/grid/cell", new UpdateCellColorDto
        {
            Q = 3,
            R = 3,
            TeamColor = TeamColor.Blue,
            HexColor = "#0000ff"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateCellColor_ShouldAllowAdminToUpdateAnyTeam()
    {
        var token = await SeedAdminAndLoginAsync("ApiAdmin", "pw12345");
        await SeedCellsAsync(new Cell { Q = 4, R = 7, TeamColor = TeamColor.Blue, HexColor = "#0000aa" });

        SetBearerToken(Client, token);

        var response = await Client.PutAsJsonAsync("/api/grid/cell", new UpdateCellColorDto
        {
            Q = 4,
            R = 7,
            TeamColor = TeamColor.Blue,
            HexColor = "#0000ee"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updated = await FindCellAsync(4, 7, TeamColor.Blue);
        updated.Should().NotBeNull();

        if (updated is null)
        {
            throw new InvalidOperationException("Updated cell was not found.");
        }

        updated.HexColor.Should().Be("#0000ee");
    }

    [Fact]
    public async Task UpdateCellColor_ShouldReturnBadRequest_WhenSpectrumIsInvalid()
    {
        var token = await RegisterAndLoginAsync("SpectrumUser", "pw12345", TeamColor.Red);
        await SeedCellsAsync(new Cell { Q = 8, R = 1, TeamColor = TeamColor.Red, HexColor = "#ff1111" });

        SetBearerToken(Client, token);

        var response = await Client.PutAsJsonAsync("/api/grid/cell", new UpdateCellColorDto
        {
            Q = 8,
            R = 1,
            TeamColor = TeamColor.Red,
            HexColor = "#0000ff"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Wrong color spectrum");
    }
}
