using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class fr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FridgeRequestItems_FridgesInventory_InventoryId",
                table: "FridgeRequestItems");

            migrationBuilder.DropIndex(
                name: "IX_FridgeRequestItems_InventoryId",
                table: "FridgeRequestItems");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "FridgeRequestItems");

            migrationBuilder.AddColumn<string>(
                name: "FridgeName",
                table: "FridgeRequestItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FridgeName",
                table: "FridgeRequestItems");

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "FridgeRequestItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FridgeRequestItems_InventoryId",
                table: "FridgeRequestItems",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FridgeRequestItems_FridgesInventory_InventoryId",
                table: "FridgeRequestItems",
                column: "InventoryId",
                principalTable: "FridgesInventory",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
