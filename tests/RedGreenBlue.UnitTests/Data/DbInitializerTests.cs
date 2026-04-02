using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RedGreenBlue.Data;
using RedGreenBlue.Models;
using RedGreenBlue.Services.Interfaces;
using RedGreenBlue.UnitTests.Infrastructure;

namespace RedGreenBlue.UnitTests.Data;

public class DbInitializerTests
{
    [Fact]
    public async Task SeedDefaultUsersAsync_ShouldCreateMissingUsersWithHashedPasswords()
    {
        await using var db = await SqliteTestDb.CreateAsync();
        var passwordHasher = new Mock<IPasswordHasher>();

        passwordHasher.Setup(h => h.GenerateSalt()).Returns("generated-salt");
        passwordHasher
            .Setup(h => h.HashPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string plain, string _) => $"hash::{plain}");

        await DbInitializer.SeedDefaultUsersAsync(db.Context, passwordHasher.Object);

        var users = await db.Context.Users.OrderBy(u => u.Username).ToListAsync();

        users.Should().HaveCount(4);
        users.Should().OnlyContain(u => u.Password.StartsWith("hash::", StringComparison.Ordinal));
        users.Should().OnlyContain(u => u.Password != "pw123");
        users.Should().Contain(u => u.Username == "Admin" && u.IsAdmin);
    }

    [Fact]
    public async Task SeedDefaultUsersAsync_ShouldBeIdempotent()
    {
        await using var db = await SqliteTestDb.CreateAsync();
        var passwordHasher = new Mock<IPasswordHasher>();

        passwordHasher.Setup(h => h.GenerateSalt()).Returns("salt");
        passwordHasher
            .Setup(h => h.HashPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string plain, string _) => $"hash::{plain}");

        await DbInitializer.SeedDefaultUsersAsync(db.Context, passwordHasher.Object);
        await DbInitializer.SeedDefaultUsersAsync(db.Context, passwordHasher.Object);

        var count = await db.Context.Users.CountAsync();
        count.Should().Be(4);

        passwordHasher.Verify(h => h.HashPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(4));
    }

    [Fact]
    public async Task SeedRectangleForAllTeamsAsync_ShouldCreate100By100GridPerTeam()
    {
        await using var db = await SqliteTestDb.CreateAsync();

        await DbInitializer.SeedRectangleForAllTeamsAsync(db.Context);

        var total = await db.Context.Cells.CountAsync();
        var red = await db.Context.Cells.CountAsync(c => c.TeamColor == TeamColor.Red);
        var green = await db.Context.Cells.CountAsync(c => c.TeamColor == TeamColor.Green);
        var blue = await db.Context.Cells.CountAsync(c => c.TeamColor == TeamColor.Blue);

        total.Should().Be(30000);
        red.Should().Be(10000);
        green.Should().Be(10000);
        blue.Should().Be(10000);
    }

    [Fact]
    public async Task SeedRectangleForAllTeamsAsync_ShouldSkip_WhenCellsAlreadyExist()
    {
        await using var db = await SqliteTestDb.CreateAsync();

        await db.Context.Cells.AddAsync(new Cell
        {
            Q = 0,
            R = 0,
            TeamColor = TeamColor.Red,
            HexColor = "#ff0000"
        });
        await db.Context.SaveChangesAsync();

        await DbInitializer.SeedRectangleForAllTeamsAsync(db.Context);

        var total = await db.Context.Cells.CountAsync();
        total.Should().Be(1);
    }
}
