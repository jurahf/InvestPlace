using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestPlaceDB.Migrations
{
    public partial class SellerExchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueryForExchange_Pazzle_PazzleId",
                table: "QueryForExchange");

            migrationBuilder.AlterColumn<int>(
                name: "PazzleId",
                table: "QueryForExchange",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LotId",
                table: "QueryForExchange",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QueryForExchange_LotId",
                table: "QueryForExchange",
                column: "LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_QueryForExchange_Lot_LotId",
                table: "QueryForExchange",
                column: "LotId",
                principalTable: "Lot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QueryForExchange_Pazzle_PazzleId",
                table: "QueryForExchange",
                column: "PazzleId",
                principalTable: "Pazzle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueryForExchange_Lot_LotId",
                table: "QueryForExchange");

            migrationBuilder.DropForeignKey(
                name: "FK_QueryForExchange_Pazzle_PazzleId",
                table: "QueryForExchange");

            migrationBuilder.DropIndex(
                name: "IX_QueryForExchange_LotId",
                table: "QueryForExchange");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "QueryForExchange");

            migrationBuilder.AlterColumn<int>(
                name: "PazzleId",
                table: "QueryForExchange",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QueryForExchange_Pazzle_PazzleId",
                table: "QueryForExchange",
                column: "PazzleId",
                principalTable: "Pazzle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
