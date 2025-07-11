using Microsoft.EntityFrameworkCore;
using RedGreenBlue.Models;

namespace RedGreenBlue.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Cell> Cells { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Eindeutiger Index für Zellenpositionen pro Team
            modelBuilder.Entity<Cell>()
                .HasIndex(c => new { c.TeamColor, c.Position })
                .IsUnique();

            // Initialdaten für 3 Teams mit je 9 Zellen
            int cellId = 1;
            foreach (TeamColor team in Enum.GetValues(typeof(TeamColor)))
            {
                for (int i = 0; i < 9; i++)
                {
                    modelBuilder.Entity<Cell>().HasData(
                        new Cell
                        {
                            Id = cellId++,
                            TeamColor = team,
                            Position = i,
                            HexColor = "#CCCCCC"
                        }
                    );
                }
            }

            // Admin-User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    Team = TeamColor.Red,
                    isAdmin = true
                }
            );
        }
    }
}