using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestPlaceDB.Migrations
{
    public partial class ExchangeFuture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lot_AspNetUsers_CompleteModeratorId",
                table: "Lot");

            migrationBuilder.DropForeignKey(
                name: "FK_Pazzle_AspNetUsers_ChangeModeratorId",
                table: "Pazzle");

            migrationBuilder.DropIndex(
                name: "IX_Pazzle_ChangeModeratorId",
                table: "Pazzle");

            migrationBuilder.DropIndex(
                name: "IX_Lot_CompleteModeratorId",
                table: "Lot");

            migrationBuilder.DropColumn(
                name: "ChangeDate",
                table: "Pazzle");

            migrationBuilder.DropColumn(
                name: "ChangeModeratorId",
                table: "Pazzle");

            migrationBuilder.DropColumn(
                name: "Changed",
                table: "Pazzle");

            migrationBuilder.DropColumn(
                name: "CompleteModerate",
                table: "Lot");

            migrationBuilder.DropColumn(
                name: "CompleteModeratorId",
                table: "Lot");

            migrationBuilder.AddColumn<int>(
                name: "CompleteNumber",
                table: "Lot",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BonusSumm",
                table: "Cash",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ExchangeLevel",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.CreateTable(
                name: "QueryForExchange",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModerateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Moderate = table.Column<bool>(nullable: true),
                    PazzleId = table.Column<int>(nullable: false),
                    ExchangeModeratorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryForExchange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueryForExchange_AspNetUsers_ExchangeModeratorId",
                        column: x => x.ExchangeModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QueryForExchange_Pazzle_PazzleId",
                        column: x => x.PazzleId,
                        principalTable: "Pazzle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QueryForExchange_ExchangeModeratorId",
                table: "QueryForExchange",
                column: "ExchangeModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryForExchange_PazzleId",
                table: "QueryForExchange",
                column: "PazzleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueryForExchange");

            migrationBuilder.DropColumn(
                name: "CompleteNumber",
                table: "Lot");

            migrationBuilder.DropColumn(
                name: "BonusSumm",
                table: "Cash");

            migrationBuilder.DropColumn(
                name: "ExchangeLevel",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeDate",
                table: "Pazzle",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChangeModeratorId",
                table: "Pazzle",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Changed",
                table: "Pazzle",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CompleteModerate",
                table: "Lot",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompleteModeratorId",
                table: "Lot",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pazzle_ChangeModeratorId",
                table: "Pazzle",
                column: "ChangeModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Lot_CompleteModeratorId",
                table: "Lot",
                column: "CompleteModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lot_AspNetUsers_CompleteModeratorId",
                table: "Lot",
                column: "CompleteModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pazzle_AspNetUsers_ChangeModeratorId",
                table: "Pazzle",
                column: "ChangeModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
