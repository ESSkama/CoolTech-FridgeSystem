using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class customerfridgesandwarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "CustomerFridges",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFridges_WarehouseId",
                table: "CustomerFridges",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFridges_Warehouses_WarehouseId",
                table: "CustomerFridges",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFridges_Warehouses_WarehouseId",
                table: "CustomerFridges");

            migrationBuilder.DropIndex(
                name: "IX_CustomerFridges_WarehouseId",
                table: "CustomerFridges");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "CustomerFridges");
        }
    }
}
