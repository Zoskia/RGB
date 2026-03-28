using Microsoft.EntityFrameworkCore;
using RedGreenBlue.Models;
using RedGreenBlue.Services.Interfaces;

namespace RedGreenBlue.Data;

public static class DbInitializer
{
    private static readonly (string Username, string Password, TeamColor Team, bool IsAdmin)[] DefaultUsers =
    [
        ("Admin", "pw123", TeamColor.Red, true),
        ("RedUser", "pw123", TeamColor.Red, false),
        ("GreenUser", "pw123", TeamColor.Green, false),
        ("BlueUser", "pw123", TeamColor.Blue, false)
    ];

    public static async Task SeedDefaultUsersAsync(
        ApplicationDbContext db,
        IPasswordHasher passwordHasher)
    {
        var existingUsernames = await db.Users
            .Select(u => u.Username)
            .ToListAsync();

        var existing = existingUsernames
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var usersToCreate = new List<User>();

        foreach (var defaultUser in DefaultUsers)
        {
            if (existing.Contains(defaultUser.Username))
            {
                continue;
            }

            var salt = passwordHasher.GenerateSalt();
            var hash = passwordHasher.HashPassword(defaultUser.Password, salt);

            usersToCreate.Add(new User
            {
                Username = defaultUser.Username,
                Password = hash,
                Salt = salt,
                Team = defaultUser.Team,
                IsAdmin = defaultUser.IsAdmin
            });
        }

        if (usersToCreate.Count == 0)
        {
            return;
        }

        await db.Users.AddRangeAsync(usersToCreate);
        await db.SaveChangesAsync();
    }

    public static async Task SeedRectangleForAllTeamsAsync(
        ApplicationDbContext db)
    {
        const int width = 100;
        const int height = 100;

        if (await db.Cells.AnyAsync()) return;

        var buffer = new List<Cell>(width * height * 3);

        for (int r = 0; r < height; r++)
        {
            int startQ = -(r / 2);
            for (int i = 0; i < width; i++)
            {
                int q = startQ + i;

                buffer.Add(new Cell { Q = q, R = r, TeamColor = TeamColor.Red, HexColor = "#b87a7aff" });
                buffer.Add(new Cell { Q = q, R = r, TeamColor = TeamColor.Green, HexColor = "#76ab76ff" });
                buffer.Add(new Cell { Q = q, R = r, TeamColor = TeamColor.Blue, HexColor = "#8484bfff" });
            }
        }

        await db.Cells.AddRangeAsync(buffer);
        await db.SaveChangesAsync();
    }
}
