using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerName = table.Column<string>(type: "TEXT", nullable: false),
                    Tagline = table.Column<string>(type: "TEXT", nullable: false),
                    Eyebrow = table.Column<string>(type: "TEXT", nullable: false),
                    HeroCta = table.Column<string>(type: "TEXT", nullable: false),
                    HeroSecondaryCta = table.Column<string>(type: "TEXT", nullable: false),
                    ContactBlurb = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    GitHubUrl = table.Column<string>(type: "TEXT", nullable: true),
                    LinkedInUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteProfiles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SiteProfiles",
                columns: new[] { "Id", "ContactBlurb", "Email", "Eyebrow", "GitHubUrl", "HeroCta", "HeroSecondaryCta", "LinkedInUrl", "Location", "OwnerName", "Tagline" },
                values: new object[] { 1, "Have an opportunity or just want to say hi? Fill out the form and I'll get back to you.", "hello@example.com", "👋 Welcome to my portfolio", null, "Hire Me", "View Projects", null, "Your City, Country", "Your Name", "Full Stack Developer · .NET · Blazor · Cloud" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteProfiles");
        }
    }
}
