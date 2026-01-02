using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AedLocator.Migrations
{
    public partial class AddLatLng : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Aeds",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Aeds",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Aeds");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Aeds");
        }
    }
}
