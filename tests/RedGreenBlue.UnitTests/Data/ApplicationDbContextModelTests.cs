using FluentAssertions;
using RedGreenBlue.Models;
using RedGreenBlue.UnitTests.Infrastructure;

namespace RedGreenBlue.UnitTests.Data;

public class ApplicationDbContextModelTests
{
    [Fact]
    public async Task Model_ShouldConfigureCompositePrimaryKey_ForCell()
    {
        await using var db = await SqliteTestDb.CreateAsync();

        var entity = db.Context.Model.FindEntityType(typeof(Cell));
        var key = entity!.FindPrimaryKey();

        key.Should().NotBeNull();
        key!.Properties.Select(p => p.Name).Should().ContainInOrder("Q", "R", "TeamColor");
    }

    [Fact]
    public async Task Model_ShouldConfigureUniqueIndex_ForUsername()
    {
        await using var db = await SqliteTestDb.CreateAsync();

        var entity = db.Context.Model.FindEntityType(typeof(User));
        var indexes = entity!.GetIndexes().ToList();

        indexes.Should().Contain(i => i.IsUnique && i.Properties.Select(p => p.Name).SequenceEqual(new[] { "Username" }));
    }
}
