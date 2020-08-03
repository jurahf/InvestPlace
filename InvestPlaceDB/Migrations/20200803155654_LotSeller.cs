using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestPlaceDB.Migrations
{
    public partial class LotSeller : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Seller_SellerId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Lot_Seller_SellerId",
                table: "Lot");

            migrationBuilder.DropTable(
                name: "Seller");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SellerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_Lot_AspNetUsers_SellerId",
                table: "Lot",
                column: "SellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lot_AspNetUsers_SellerId",
                table: "Lot");

            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Seller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contract = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ContractModeratorId = table.Column<int>(type: "int", nullable: true),
                    ContrectModerate = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seller", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seller_AspNetUsers_ContractModeratorId",
                        column: x => x.ContractModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SellerId",
                table: "AspNetUsers",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Seller_ContractModeratorId",
                table: "Seller",
                column: "ContractModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Seller_SellerId",
                table: "AspNetUsers",
                column: "SellerId",
                principalTable: "Seller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lot_Seller_SellerId",
                table: "Lot",
                column: "SellerId",
                principalTable: "Seller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
