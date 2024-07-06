using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviePro.Migrations
{
    public partial class _002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Movie",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MyRating",
                table: "Movie",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "MyRating",
                table: "Movie");
        }
    }
}
