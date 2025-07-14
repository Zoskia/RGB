using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedGreenBlue.Migrations
{
    /// <inheritdoc />
    public partial class IndexRemovedInCellTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cells_TeamColor_Position",
                table: "Cells");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cells_TeamColor_Position",
                table: "Cells",
                columns: new[] { "TeamColor", "Position" },
                unique: true);
        }
    }
}
