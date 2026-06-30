using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectAndBoardContextToActivityLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BoardId",
                table: "ActivityLogs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoardName",
                table: "ActivityLogs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "ActivityLogs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "ActivityLogs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardId",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "BoardName",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "ActivityLogs");
        }
    }
}
