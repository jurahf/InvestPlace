using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestPlaceDB.Migrations
{
    public partial class CashInputOutputByUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CashQueryClientProcessorId",
                table: "QueryForOperation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "QueryForOperation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_QueryForOperation_CashQueryClientProcessorId",
                table: "QueryForOperation",
                column: "CashQueryClientProcessorId");

            migrationBuilder.AddForeignKey(
                name: "FK_QueryForOperation_AspNetUsers_CashQueryClientProcessorId",
                table: "QueryForOperation",
                column: "CashQueryClientProcessorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueryForOperation_AspNetUsers_CashQueryClientProcessorId",
                table: "QueryForOperation");

            migrationBuilder.DropIndex(
                name: "IX_QueryForOperation_CashQueryClientProcessorId",
                table: "QueryForOperation");

            migrationBuilder.DropColumn(
                name: "CashQueryClientProcessorId",
                table: "QueryForOperation");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "QueryForOperation");
        }
    }
}
