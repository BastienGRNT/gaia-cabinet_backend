using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gaiacabinet_api.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshSessionsRevokedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RevokedByUserId",
                table: "RefreshSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_RevokedByUserId",
                table: "RefreshSessions",
                column: "RevokedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshSessions_Users_RevokedByUserId",
                table: "RefreshSessions",
                column: "RevokedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshSessions_Users_RevokedByUserId",
                table: "RefreshSessions");

            migrationBuilder.DropIndex(
                name: "IX_RefreshSessions_RevokedByUserId",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "RevokedByUserId",
                table: "RefreshSessions");
        }
    }
}
