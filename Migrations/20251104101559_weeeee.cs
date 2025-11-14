using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class weeeee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerFridges_SerialNumber",
                table: "CustomerFridges");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "CustomerFridges",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFridges_SerialNumber",
                table: "CustomerFridges",
                column: "SerialNumber",
                unique: true,
                filter: "[SerialNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerFridges_SerialNumber",
                table: "CustomerFridges");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "CustomerFridges",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFridges_SerialNumber",
                table: "CustomerFridges",
                column: "SerialNumber",
                unique: true);
        }
    }
}
