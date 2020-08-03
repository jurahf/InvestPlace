using Microsoft.EntityFrameworkCore.Migrations;
using System.Linq;

namespace InvestPlaceDB.Migrations
{
    public partial class CreatePriceRange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriceRangeId",
                table: "Lot",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PriceRange",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Minimum = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Maximum = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceRange", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lot_PriceRangeId",
                table: "Lot",
                column: "PriceRangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lot_PriceRange_PriceRangeId",
                table: "Lot",
                column: "PriceRangeId",
                principalTable: "PriceRange",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);


            object[][] firstStep = Enumerable.Range(1, 20).Select(x => new object[] { (x - 1) * 50 + 1, x * 50 }).ToArray();
            object[][] secondStep = Enumerable.Range(1, 20).Select(x => new object[] { (x - 1) * 100 + 1001, x * 100 + 1000 }).ToArray();
            object[][] thirdStep = Enumerable.Range(1, 10).Select(x => new object[] { (x - 1) * 500 + 5001, x * 500 + 5000 }).ToArray();
            object[][] fourthStep = Enumerable.Range(1, 10).Select(x => new object[] { (x - 1) * 1000 + 10_001, x * 1000 + 10_000 }).ToArray();
            object[][] allSteps = firstStep.Concat(secondStep).Concat(thirdStep).Concat(fourthStep).ToArray();
            object[,] result = new object[60, 2];
            for (int i = 0; i < allSteps.Length; i++)
            {
                for (int j = 0; j < allSteps[i].Length; j++)
                {
                    result[i, j] = allSteps[i][j];
                }
            }

            migrationBuilder.InsertData(
                table: "PriceRange",
                columns: new[] { "Minimum", "Maximum" },
                values: result);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lot_PriceRange_PriceRangeId",
                table: "Lot");

            migrationBuilder.DropTable(
                name: "PriceRange");

            migrationBuilder.DropIndex(
                name: "IX_Lot_PriceRangeId",
                table: "Lot");

            migrationBuilder.DropColumn(
                name: "PriceRangeId",
                table: "Lot");
        }
    }
}
