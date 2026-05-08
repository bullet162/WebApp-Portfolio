using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlazorPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class AddDeveloperNetworkRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AiEnrichmentConsent",
                table: "CollaborationRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "AiEnrichmentConsentAt",
                table: "CollaborationRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BadgeEnabled",
                table: "CollaborationRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ConnectionType",
                table: "CollaborationRequests",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "CollaborationRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditTokenExpiresAt",
                table: "CollaborationRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditTokenHash",
                table: "CollaborationRequests",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditTokenUsedAt",
                table: "CollaborationRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHubUrl",
                table: "CollaborationRequests",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "CollaborationRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "CollaborationRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "CollaborationRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAiEnrichmentAt",
                table: "CollaborationRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditRequestedAt",
                table: "CollaborationRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditedAt",
                table: "CollaborationRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "CollaborationRequests",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OpenToCollaborate",
                table: "CollaborationRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PublicSlug",
                table: "CollaborationRequests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReciprocalLinkCheckError",
                table: "CollaborationRequests",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReciprocalLinkLastCheckedAt",
                table: "CollaborationRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReciprocalLinkVerified",
                table: "CollaborationRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReciprocalLinkVerifiedAt",
                table: "CollaborationRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewNote",
                table: "CollaborationRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedBy",
                table: "CollaborationRequests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleTitle",
                table: "CollaborationRequests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "CollaborationRequests",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CollaborationRequests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "DeveloperNetworkProfileRevisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CollaborationRequestId = table.Column<int>(type: "integer", nullable: false),
                    ProposedFirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProposedLastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProposedPortfolioUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ProposedMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ProposedRoleTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProposedConnectionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProposedTags = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProposedGitHubUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ProposedLinkedInUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ProposedOpenToCollaborate = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReviewNote = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeveloperNetworkProfileRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeveloperNetworkProfileRevisions_CollaborationRequests_Coll~",
                        column: x => x.CollaborationRequestId,
                        principalTable: "CollaborationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeveloperProfileEnrichments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CollaborationRequestId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    GeneratedHeadline = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    GeneratedSummary = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    GeneratedSkillsJson = table.Column<string>(type: "text", nullable: true),
                    GeneratedProjectHighlightsJson = table.Column<string>(type: "text", nullable: true),
                    GeneratedCollaborationInterestsJson = table.Column<string>(type: "text", nullable: true),
                    SourceCitationsJson = table.Column<string>(type: "text", nullable: true),
                    ConfidenceScore = table.Column<double>(type: "double precision", nullable: false),
                    ModelUsed = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PromptVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeveloperProfileEnrichments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeveloperProfileEnrichments_CollaborationRequests_Collabora~",
                        column: x => x.CollaborationRequestId,
                        principalTable: "CollaborationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Company", "Description", "Title" },
                values: new object[] { "Professional Experience", "Developing scalable web solutions and backend systems.", "Software Developer" });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Company", "Description" },
                values: new object[] { "Freelance / Open Source", "Built and maintained high-performance web applications using .NET and modern frameworks." });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "TechStack" },
                values: new object[] { "Problem: Building a personal brand that is easy to manage. Solution: A custom-built CMS and portfolio. Tech: Blazor Server, EF Core, Neon PostgreSQL. Result: A professional, dynamic portfolio.", "Blazor, EF Core, PostgreSQL" });

            migrationBuilder.UpdateData(
                table: "SiteProfiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Location", "OwnerName" },
                values: new object[] { "hello@jhersonaguto.dev", "Philippines", "Jherson Aguto" });

            migrationBuilder.CreateIndex(
                name: "IX_DeveloperNetworkProfileRevisions_CollaborationRequestId",
                table: "DeveloperNetworkProfileRevisions",
                column: "CollaborationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DeveloperProfileEnrichments_CollaborationRequestId",
                table: "DeveloperProfileEnrichments",
                column: "CollaborationRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeveloperNetworkProfileRevisions");

            migrationBuilder.DropTable(
                name: "DeveloperProfileEnrichments");

            migrationBuilder.DropColumn(
                name: "AiEnrichmentConsent",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "AiEnrichmentConsentAt",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "BadgeEnabled",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "ConnectionType",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "EditTokenExpiresAt",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "EditTokenHash",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "EditTokenUsedAt",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "GitHubUrl",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "LastAiEnrichmentAt",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "LastEditRequestedAt",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "LastEditedAt",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "OpenToCollaborate",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "PublicSlug",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "ReciprocalLinkCheckError",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "ReciprocalLinkLastCheckedAt",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "ReciprocalLinkVerified",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "ReciprocalLinkVerifiedAt",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "ReviewNote",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "RoleTitle",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "CollaborationRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CollaborationRequests");

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Company", "Description", "Title" },
                values: new object[] { "Acme Corp", "Led development of cloud-native applications.", "Senior Developer" });

            migrationBuilder.UpdateData(
                table: "Experiences",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Company", "Description" },
                values: new object[] { "Startup Inc", "Built and maintained web applications using .NET and React." });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "TechStack" },
                values: new object[] { "This very portfolio, built with Blazor and EF Core.", "Blazor, EF Core, SQLite" });

            migrationBuilder.UpdateData(
                table: "SiteProfiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "Location", "OwnerName" },
                values: new object[] { "hello@example.com", "Your City, Country", "Your Name" });
        }
    }
}
