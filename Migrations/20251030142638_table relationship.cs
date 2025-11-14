using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class tablerelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FridgeInventoryInventoryId",
                table: "WarehouseStocks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseStocks_FridgeInventoryInventoryId",
                table: "WarehouseStocks",
                column: "FridgeInventoryInventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseStocks_FridgesInventory_FridgeInventoryInventoryId",
                table: "WarehouseStocks",
                column: "FridgeInventoryInventoryId",
                principalTable: "FridgesInventory",
                principalColumn: "InventoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseStocks_FridgesInventory_FridgeInventoryInventoryId",
                table: "WarehouseStocks");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseStocks_FridgeInventoryInventoryId",
                table: "WarehouseStocks");

            migrationBuilder.DropColumn(
                name: "FridgeInventoryInventoryId",
                table: "WarehouseStocks");
        }
    }
}
