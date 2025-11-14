using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class custmerfridge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerFridgeId",
                table: "FridgeRequestItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FridgeRequestItems_CustomerFridgeId",
                table: "FridgeRequestItems",
                column: "CustomerFridgeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FridgeRequestItems_CustomerFridges_CustomerFridgeId",
                table: "FridgeRequestItems",
                column: "CustomerFridgeId",
                principalTable: "CustomerFridges",
                principalColumn: "CustomerFridgeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FridgeRequestItems_CustomerFridges_CustomerFridgeId",
                table: "FridgeRequestItems");

            migrationBuilder.DropIndex(
                name: "IX_FridgeRequestItems_CustomerFridgeId",
                table: "FridgeRequestItems");

            migrationBuilder.DropColumn(
                name: "CustomerFridgeId",
                table: "FridgeRequestItems");
        }
    }
}
