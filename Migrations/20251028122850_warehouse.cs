using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class warehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFridges_Fridges_FridgeId",
                table: "CustomerFridges");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "Warehouses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Warehouses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FridgeImage",
                table: "Fridges",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFridges_Fridges_FridgeId",
                table: "CustomerFridges",
                column: "FridgeId",
                principalTable: "Fridges",
                principalColumn: "FridgeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerFridges_Fridges_FridgeId",
                table: "CustomerFridges");

            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Warehouses");

            migrationBuilder.AlterColumn<string>(
                name: "FridgeImage",
                table: "Fridges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerFridges_Fridges_FridgeId",
                table: "CustomerFridges",
                column: "FridgeId",
                principalTable: "Fridges",
                principalColumn: "FridgeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
