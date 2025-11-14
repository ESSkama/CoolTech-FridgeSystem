using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class PurchaseRequesttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Employees_ManagerId",
                table: "PurchaseRequests");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequests_ManagerId",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "PurchaseRequests");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "PurchaseRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_EmployeeId",
                table: "PurchaseRequests",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Employees_EmployeeId",
                table: "PurchaseRequests",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Employees_EmployeeId",
                table: "PurchaseRequests");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequests_EmployeeId",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "PurchaseRequests");

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "PurchaseRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_ManagerId",
                table: "PurchaseRequests",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Employees_ManagerId",
                table: "PurchaseRequests",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
