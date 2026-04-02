using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RedGreenBlue.Dtos;
using RedGreenBlue.Models;
using RedGreenBlue.Repositories;
using RedGreenBlue.UnitTests.Infrastructure;

namespace RedGreenBlue.UnitTests.Repositories;

public class GridRepositoryTests
{
    [Fact]
    public async Task GetCellsAsync_ShouldReturnOnlyRequestedTeamCells()
    {
        await using var db = await SqliteTestDb.CreateAsync();
        await db.Context.Cells.AddRangeAsync(
            new Cell { Q = 0, R = 0, TeamColor = TeamColor.Red, HexColor = "#ff0000" },
            new Cell { Q = 1, R = 0, TeamColor = TeamColor.Red, HexColor = "#cc0000" },
            new Cell { Q = 0, R = 0, TeamColor = TeamColor.Blue, HexColor = "#0000ff" });
        await db.Context.SaveChangesAsync();

        var repository = new GridRepository(db.Context);

        var result = await repository.GetCellsAsync(TeamColor.Red);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.TeamColor == TeamColor.Red);
    }

    [Fact]
    public async Task UpdateCellColorAsync_ShouldUpdateExistingCell()
    {
        await using var db = await SqliteTestDb.CreateAsync();
        await db.Context.Cells.AddAsync(new Cell
        {
            Q = 4,
            R = 6,
            TeamColor = TeamColor.Green,
            HexColor = "#00aa00"
        });
        await db.Context.SaveChangesAsync();

        var repository = new GridRepository(db.Context);

        var result = await repository.UpdateCellColorAsync(new UpdateCellColorDto
        {
            Q = 4,
            R = 6,
            TeamColor = TeamColor.Green,
            HexColor = "#00ff00"
        });

        result.Should().BeTrue();

        var updated = await db.Context.Cells.SingleAsync(c => c.Q == 4 && c.R == 6 && c.TeamColor == TeamColor.Green);
        updated.HexColor.Should().Be("#00ff00");
    }

    [Fact]
    public async Task UpdateCellColorAsync_ShouldReturnFalse_WhenCellDoesNotExist()
    {
        await using var db = await SqliteTestDb.CreateAsync();
        var repository = new GridRepository(db.Context);

        var result = await repository.UpdateCellColorAsync(new UpdateCellColorDto
        {
            Q = 99,
            R = 99,
            TeamColor = TeamColor.Blue,
            HexColor = "#0000ff"
        });

        result.Should().BeFalse();
        var cellCount = await db.Context.Cells.CountAsync();
        cellCount.Should().Be(0);
    }
}
