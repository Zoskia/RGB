using Microsoft.EntityFrameworkCore;
using RedGreenBlue.Models;

namespace RedGreenBlue.Data;

public static class DbInitializer
{
    public static async Task SeedRectangleForAllTeamsAsync(
        ApplicationDbContext db,
        int width,
        int height)
    {
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
