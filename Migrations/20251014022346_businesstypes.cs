using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class businesstypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessType",
                table: "Customers");

            migrationBuilder.AddColumn<int>(
                name: "BusinessTypeId",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BusinessTypes",
                columns: table => new
                {
                    BusinessTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessTypes", x => x.BusinessTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_BusinessTypeId",
                table: "Customers",
                column: "BusinessTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_BusinessTypes_BusinessTypeId",
                table: "Customers",
                column: "BusinessTypeId",
                principalTable: "BusinessTypes",
                principalColumn: "BusinessTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_BusinessTypes_BusinessTypeId",
                table: "Customers");

            migrationBuilder.DropTable(
                name: "BusinessTypes");

            migrationBuilder.DropIndex(
                name: "IX_Customers_BusinessTypeId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BusinessTypeId",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "BusinessType",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
