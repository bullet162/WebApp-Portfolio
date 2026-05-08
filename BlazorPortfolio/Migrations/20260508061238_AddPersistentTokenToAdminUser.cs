using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class AddPersistentTokenToAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PersistentToken",
                table: "AdminUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PersistentTokenExpiresAt",
                table: "AdminUsers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersistentToken",
                table: "AdminUsers");

            migrationBuilder.DropColumn(
                name: "PersistentTokenExpiresAt",
                table: "AdminUsers");
        }
    }
}
