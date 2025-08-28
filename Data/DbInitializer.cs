using System;
using Microsoft.EntityFrameworkCore;
using RedGreenBlue.Models;

namespace RedGreenBlue.Data;

public static class DbInitializer
{
    public static async Task SeedRectangleForAllTeamsAsync(
        ApplicationDbContext db,
        int width,
        int height,
        string hexColor = "#cccccc")
    {
        if (await db.Cells.AnyAsync()) return;

        var buffer = new List<Cell>(width * height * 3);

        for (int r = 0; r < height; r++)
        {
            int startQ = -(r / 2); // ⌊r/2⌋
            for (int i = 0; i < width; i++)
            {
                int q = startQ + i;

                buffer.Add(new Cell { Q = q, R = r, TeamColor = TeamColor.Red, HexColor = hexColor });
                buffer.Add(new Cell { Q = q, R = r, TeamColor = TeamColor.Green, HexColor = hexColor });
                buffer.Add(new Cell { Q = q, R = r, TeamColor = TeamColor.Blue, HexColor = hexColor });
            }
        }

        await db.Cells.AddRangeAsync(buffer);
        await db.SaveChangesAsync();
    }
}
