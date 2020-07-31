using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestPlaceDB.Migrations
{
    public partial class InnerUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InnerName",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InnerName",
                table: "AspNetUsers");
        }
    }
}
