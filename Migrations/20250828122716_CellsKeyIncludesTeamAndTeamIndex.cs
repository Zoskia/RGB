using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedGreenBlue.Migrations
{
    /// <inheritdoc />
    public partial class CellsKeyIncludesTeamAndTeamIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Cells",
                table: "Cells");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cells",
                table: "Cells",
                columns: new[] { "Q", "R", "TeamColor" });

            migrationBuilder.CreateIndex(
                name: "IX_Cells_TeamColor_R_Q",
                table: "Cells",
                columns: new[] { "TeamColor", "R", "Q" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Cells",
                table: "Cells");

            migrationBuilder.DropIndex(
                name: "IX_Cells_TeamColor_R_Q",
                table: "Cells");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cells",
                table: "Cells",
                columns: new[] { "Q", "R" });
        }
    }
}
