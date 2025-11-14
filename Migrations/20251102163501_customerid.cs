using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class customerid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "CustomerFridges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFridges_CustomerId",
                table: "CustomerFridges",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFridges_Customers_CustomerId",
                table: "CustomerFridges",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "BusinessId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFridges_Customers_CustomerId",
                table: "CustomerFridges");

            migrationBuilder.DropIndex(
                name: "IX_CustomerFridges_CustomerId",
                table: "CustomerFridges");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "CustomerFridges");
        }
    }
}
