using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReFound.Migrations
{
    /// <inheritdoc />
    public partial class AddLostItemPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "LostItemReports",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "LostItemReports");
        }
    }
}
