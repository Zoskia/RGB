using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RedGreenBlue.Models;
using RedGreenBlue.Repositories;
using RedGreenBlue.UnitTests.Infrastructure;

namespace RedGreenBlue.UnitTests.Repositories;

public class UserRepositoryTests
{
    [Fact]
    public async Task UsernameAvailableAsync_ShouldReturnTrue_WhenUsernameIsNotUsed()
    {
        await using var db = await SqliteTestDb.CreateAsync();
        var repository = new UserRepository(db.Context);

        var result = await repository.UsernameAvailableAsync("FreshUser");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task AddNewUserAsync_ShouldPersistUser_WhenUsernameIsAvailable()
    {
        await using var db = await SqliteTestDb.CreateAsync();
        var repository = new UserRepository(db.Context);

        var user = new User
        {
            Username = "Alice1",
            Password = "hash",
            Salt = "salt",
            Team = TeamColor.Red,
            IsAdmin = false
        };

        var result = await repository.AddNewUserAsync(user);

        result.Should().NotBeNull();
        result!.Id.Should().BeGreaterThan(0);

        var fromDb = await db.Context.Users.SingleAsync();
        fromDb.Username.Should().Be("Alice1");
        fromDb.Team.Should().Be(TeamColor.Red);
    }

    [Fact]
    public async Task AddNewUserAsync_ShouldReturnNull_WhenUsernameAlreadyExists()
    {
        await using var db = await SqliteTestDb.CreateAsync();
        await db.Context.Users.AddAsync(new User
        {
            Username = "TakenUser",
            Password = "hash",
            Salt = "salt",
            Team = TeamColor.Green
        });
        await db.Context.SaveChangesAsync();

        var repository = new UserRepository(db.Context);

        var result = await repository.AddNewUserAsync(new User
        {
            Username = "TakenUser",
            Password = "otherHash",
            Salt = "otherSalt",
            Team = TeamColor.Blue
        });

        result.Should().BeNull();
        var count = await db.Context.Users.CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnUser_WhenUsernameExists()
    {
        await using var db = await SqliteTestDb.CreateAsync();
        await db.Context.Users.AddAsync(new User
        {
            Username = "LookupUser",
            Password = "hash",
            Salt = "salt",
            Team = TeamColor.Blue
        });
        await db.Context.SaveChangesAsync();

        var repository = new UserRepository(db.Context);

        var result = await repository.GetByUsernameAsync("LookupUser");

        result.Should().NotBeNull();
        result!.Username.Should().Be("LookupUser");
    }
}
