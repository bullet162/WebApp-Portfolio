using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class AddGitHubUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GitHubUsername",
                table: "SiteProfiles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SiteProfiles",
                keyColumn: "Id",
                keyValue: 1,
                column: "GitHubUsername",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitHubUsername",
                table: "SiteProfiles");
        }
    }
}
