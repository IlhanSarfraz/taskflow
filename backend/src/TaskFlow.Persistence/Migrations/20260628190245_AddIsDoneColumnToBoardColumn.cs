using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDoneColumnToBoardColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDoneColumn",
                table: "BoardColumns",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDoneColumn",
                table: "BoardColumns");
        }
    }
}
