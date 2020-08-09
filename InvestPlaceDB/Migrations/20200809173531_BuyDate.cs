using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestPlaceDB.Migrations
{
    public partial class BuyDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BuyDate",
                table: "Pazzle",
                type: "datetime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyDate",
                table: "Pazzle");
        }
    }
}
