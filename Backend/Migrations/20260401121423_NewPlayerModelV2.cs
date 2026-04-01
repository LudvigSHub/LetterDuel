using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetterDuel.Backend.Migrations
{
    /// <inheritdoc />
    public partial class NewPlayerModelV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JoinOrder",
                table: "Players",
                newName: "PlayerNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlayerNumber",
                table: "Players",
                newName: "JoinOrder");
        }
    }
}
