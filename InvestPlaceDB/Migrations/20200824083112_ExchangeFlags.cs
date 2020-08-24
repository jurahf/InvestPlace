using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestPlaceDB.Migrations
{
    public partial class ExchangeFlags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExchangeLevelPercent",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "ExchangeByBuyer",
                table: "Lot",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExchangeBySeller",
                table: "Lot",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExchangeByBuyer",
                table: "Lot");

            migrationBuilder.DropColumn(
                name: "ExchangeBySeller",
                table: "Lot");

            migrationBuilder.AddColumn<int>(
                name: "ExchangeLevelPercent",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }
    }
}
