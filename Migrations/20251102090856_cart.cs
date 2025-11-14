using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class cart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FridgeRequests_FridgesInventory_InventoryId",
                table: "FridgeRequests");

            migrationBuilder.DropIndex(
                name: "IX_FridgeRequests_InventoryId",
                table: "FridgeRequests");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "FridgeRequests");

            migrationBuilder.AddColumn<int>(
                name: "FridgeInventoryInventoryId",
                table: "FridgeRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "FridgeRequestItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FridgeRequests_FridgeInventoryInventoryId",
                table: "FridgeRequests",
                column: "FridgeInventoryInventoryId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_FridgeRequests_FridgesInventory_FridgeInventoryInventoryId",
                table: "FridgeRequests",
                column: "FridgeInventoryInventoryId",
                principalTable: "FridgesInventory",
                principalColumn: "InventoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FridgeRequestItems_FridgesInventory_InventoryId",
                table: "FridgeRequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_FridgeRequests_FridgesInventory_FridgeInventoryInventoryId",
                table: "FridgeRequests");

            migrationBuilder.DropIndex(
                name: "IX_FridgeRequests_FridgeInventoryInventoryId",
                table: "FridgeRequests");

            migrationBuilder.DropIndex(
                name: "IX_FridgeRequestItems_InventoryId",
                table: "FridgeRequestItems");

            migrationBuilder.DropColumn(
                name: "FridgeInventoryInventoryId",
                table: "FridgeRequests");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "FridgeRequestItems");

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "FridgeRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FridgeRequests_InventoryId",
                table: "FridgeRequests",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FridgeRequests_FridgesInventory_InventoryId",
                table: "FridgeRequests",
                column: "InventoryId",
                principalTable: "FridgesInventory",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
