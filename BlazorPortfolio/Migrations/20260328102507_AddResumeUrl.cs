using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResumeUrl",
                table: "SiteProfiles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SiteProfiles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ResumeUrl",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumeUrl",
                table: "SiteProfiles");
        }
    }
}
