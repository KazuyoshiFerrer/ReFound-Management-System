using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReFound.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClaimRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FoundItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClaimantUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClaimDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProofOfOwnership = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimRequests", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimRequests");
        }
    }
}
