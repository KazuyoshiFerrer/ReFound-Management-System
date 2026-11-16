using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReFound.Migrations
{
    /// <inheritdoc />
    public partial class AddFoundItemPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "FoundItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "FoundItems");
        }
    }
}
