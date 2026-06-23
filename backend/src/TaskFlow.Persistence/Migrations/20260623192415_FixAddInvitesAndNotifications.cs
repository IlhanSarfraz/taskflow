using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixAddInvitesAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Users_InvitedById",
                table: "Invites");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Invites_InvitedById",
                table: "Invites");

            migrationBuilder.DropIndex(
                name: "IX_Invites_ProjectId",
                table: "Invites");

            migrationBuilder.DropColumn(
                name: "InvitedById",
                table: "Invites");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Invites_InvitedByUserId",
                table: "Invites",
                column: "InvitedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_ProjectId_InvitedUserId_Status",
                table: "Invites",
                columns: new[] { "ProjectId", "InvitedUserId", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Users_InvitedByUserId",
                table: "Invites",
                column: "InvitedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Users_InvitedByUserId",
                table: "Invites");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId_IsRead",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Invites_InvitedByUserId",
                table: "Invites");

            migrationBuilder.DropIndex(
                name: "IX_Invites_ProjectId_InvitedUserId_Status",
                table: "Invites");

            migrationBuilder.AddColumn<Guid>(
                name: "InvitedById",
                table: "Invites",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_InvitedById",
                table: "Invites",
                column: "InvitedById");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_ProjectId",
                table: "Invites",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Users_InvitedById",
                table: "Invites",
                column: "InvitedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
