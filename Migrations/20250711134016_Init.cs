using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RedGreenBlue.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HexColor = table.Column<string>(type: "TEXT", nullable: false),
                    TeamColor = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cells", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Team = table.Column<int>(type: "INTEGER", nullable: false),
                    isAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Cells",
                columns: new[] { "Id", "HexColor", "Position", "TeamColor" },
                values: new object[,]
                {
                    { 1, "#CCCCCC", 0, 0 },
                    { 2, "#CCCCCC", 1, 0 },
                    { 3, "#CCCCCC", 2, 0 },
                    { 4, "#CCCCCC", 3, 0 },
                    { 5, "#CCCCCC", 4, 0 },
                    { 6, "#CCCCCC", 5, 0 },
                    { 7, "#CCCCCC", 6, 0 },
                    { 8, "#CCCCCC", 7, 0 },
                    { 9, "#CCCCCC", 8, 0 },
                    { 10, "#CCCCCC", 0, 1 },
                    { 11, "#CCCCCC", 1, 1 },
                    { 12, "#CCCCCC", 2, 1 },
                    { 13, "#CCCCCC", 3, 1 },
                    { 14, "#CCCCCC", 4, 1 },
                    { 15, "#CCCCCC", 5, 1 },
                    { 16, "#CCCCCC", 6, 1 },
                    { 17, "#CCCCCC", 7, 1 },
                    { 18, "#CCCCCC", 8, 1 },
                    { 19, "#CCCCCC", 0, 2 },
                    { 20, "#CCCCCC", 1, 2 },
                    { 21, "#CCCCCC", 2, 2 },
                    { 22, "#CCCCCC", 3, 2 },
                    { 23, "#CCCCCC", 4, 2 },
                    { 24, "#CCCCCC", 5, 2 },
                    { 25, "#CCCCCC", 6, 2 },
                    { 26, "#CCCCCC", 7, 2 },
                    { 27, "#CCCCCC", 8, 2 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Password", "Team", "Username", "isAdmin" },
                values: new object[] { 1, "admin123", 0, "admin", true });

            migrationBuilder.CreateIndex(
                name: "IX_Cells_TeamColor_Position",
                table: "Cells",
                columns: new[] { "TeamColor", "Position" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cells");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
