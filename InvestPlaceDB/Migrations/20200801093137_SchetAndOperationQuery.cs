using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestPlaceDB.Migrations
{
    public partial class SchetAndOperationQuery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AspNetUsersId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BIK",
                table: "AspNetUsers",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "AspNetUsers",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorrSchet",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "INN",
                table: "AspNetUsers",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KPP",
                table: "AspNetUsers",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Patronymic",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchetNumber",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "AspNetUsers",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QueryForOperation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Summ = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    CashId = table.Column<int>(nullable: false),
                    IsCashOutput = table.Column<bool>(nullable: false),
                    CashQueryModeratorId = table.Column<int>(nullable: true),
                    OperationModerate = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryForOperation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueryForOperation_Cash_CashId",
                        column: x => x.CashId,
                        principalTable: "Cash",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QueryForOperation_AspNetUsers_CashQueryModeratorId",
                        column: x => x.CashQueryModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QueryForOperation_CashId",
                table: "QueryForOperation",
                column: "CashId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryForOperation_CashQueryModeratorId",
                table: "QueryForOperation",
                column: "CashQueryModeratorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueryForOperation");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BIK",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Bank",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CorrSchet",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "INN",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "KPP",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Patronymic",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SchetNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "AspNetUsersId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }
    }
}
