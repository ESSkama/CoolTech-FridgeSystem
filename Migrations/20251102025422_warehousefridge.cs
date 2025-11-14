using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class warehousefridge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "Fridges",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fridges_WarehouseId",
                table: "Fridges",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fridges_Warehouses_WarehouseId",
                table: "Fridges",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fridges_Warehouses_WarehouseId",
                table: "Fridges");

            migrationBuilder.DropIndex(
                name: "IX_Fridges_WarehouseId",
                table: "Fridges");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Fridges");
        }
    }
}
