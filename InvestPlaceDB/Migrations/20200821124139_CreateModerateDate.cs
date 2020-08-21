using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestPlaceDB.Migrations
{
    public partial class CreateModerateDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateModerateDate",
                table: "Lot",
                type: "datetime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateModerateDate",
                table: "Lot");
        }
    }
}
