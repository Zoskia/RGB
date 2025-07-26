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
            modelBuilder.Entity<Cell>()
            .HasKey(c => new { c.Q, c.R });

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        }
    }
}