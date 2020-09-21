using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestPlaceDB.Migrations
{
    public partial class PresaveLotSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LotCategory_Lot_LotId",
                table: "LotCategory");

            migrationBuilder.AlterColumn<int>(
                name: "LotId",
                table: "LotCategory",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LotPresaveId",
                table: "LotCategory",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LotPresave",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    ImageLink = table.Column<string>(maxLength: 2000, nullable: true),
                    SourceLink = table.Column<string>(maxLength: 2000, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    SellerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotPresave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LotPresave_AspNetUsers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotCategory_LotPresaveId",
                table: "LotCategory",
                column: "LotPresaveId");

            migrationBuilder.CreateIndex(
                name: "IX_LotPresave_SellerId",
                table: "LotPresave",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_LotCategory_Lot_LotId",
                table: "LotCategory",
                column: "LotId",
                principalTable: "Lot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LotCategory_LotPresave_LotPresaveId",
                table: "LotCategory",
                column: "LotPresaveId",
                principalTable: "LotPresave",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LotCategory_Lot_LotId",
                table: "LotCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_LotCategory_LotPresave_LotPresaveId",
                table: "LotCategory");

            migrationBuilder.DropTable(
                name: "LotPresave");

            migrationBuilder.DropIndex(
                name: "IX_LotCategory_LotPresaveId",
                table: "LotCategory");

            migrationBuilder.DropColumn(
                name: "LotPresaveId",
                table: "LotCategory");

            migrationBuilder.AlterColumn<int>(
                name: "LotId",
                table: "LotCategory",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LotCategory_Lot_LotId",
                table: "LotCategory",
                column: "LotId",
                principalTable: "Lot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
