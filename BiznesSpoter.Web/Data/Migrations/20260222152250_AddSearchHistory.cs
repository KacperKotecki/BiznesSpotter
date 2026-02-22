using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiznesSpoter.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Industry = table.Column<string>(type: "TEXT", nullable: false),
                    RadiusMeters = table.Column<double>(type: "REAL", nullable: false),
                    SearchDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompetitorCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Population = table.Column<double>(type: "REAL", nullable: true),
                    CompetitionIndex = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchHistories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchHistories");
        }
    }
}
