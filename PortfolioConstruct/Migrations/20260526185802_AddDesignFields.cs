using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioConstruct.Migrations
{
    /// <inheritdoc />
    public partial class AddDesignFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccentColor",
                table: "DesignSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Theme",
                table: "DesignSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FontSize",
                table: "Block",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TextAlign",
                table: "Block",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TextColor",
                table: "Block",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccentColor",
                table: "DesignSettings");

            migrationBuilder.DropColumn(
                name: "Theme",
                table: "DesignSettings");

            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "Block");

            migrationBuilder.DropColumn(
                name: "TextAlign",
                table: "Block");

            migrationBuilder.DropColumn(
                name: "TextColor",
                table: "Block");
        }
    }
}
