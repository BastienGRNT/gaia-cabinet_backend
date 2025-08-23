using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gaiacabinet_api.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshSessionsKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserAgent",
                table: "RefreshSessions",
                newName: "LastUserAgent");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "RefreshSessions",
                newName: "LastIp");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastSeenAt",
                table: "RefreshSessions",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "NOW() AT TIME ZONE 'UTC'");

            migrationBuilder.AddColumn<string>(
                name: "SessionKeyHash",
                table: "RefreshSessions",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "RefreshSessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW() AT TIME ZONE 'UTC'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSeenAt",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "SessionKeyHash",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RefreshSessions");

            migrationBuilder.RenameColumn(
                name: "LastUserAgent",
                table: "RefreshSessions",
                newName: "UserAgent");

            migrationBuilder.RenameColumn(
                name: "LastIp",
                table: "RefreshSessions",
                newName: "IpAddress");
        }
    }
}
