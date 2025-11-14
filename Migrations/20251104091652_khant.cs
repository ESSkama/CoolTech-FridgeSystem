using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class khant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "FridgeRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CustomerFridgeId",
                table: "FaultReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FridgeFaultId",
                table: "FaultReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FaultReportId",
                table: "AssignedFaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FaultReports_CustomerFridgeId",
                table: "FaultReports",
                column: "CustomerFridgeId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultReports_FridgeFaultId",
                table: "FaultReports",
                column: "FridgeFaultId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedFaults_FaultReportId",
                table: "AssignedFaults",
                column: "FaultReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedFaults_FaultReports_FaultReportId",
                table: "AssignedFaults",
                column: "FaultReportId",
                principalTable: "FaultReports",
                principalColumn: "FaultId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaultReports_CustomerFridges_CustomerFridgeId",
                table: "FaultReports",
                column: "CustomerFridgeId",
                principalTable: "CustomerFridges",
                principalColumn: "CustomerFridgeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaultReports_FridgeFaults_FridgeFaultId",
                table: "FaultReports",
                column: "FridgeFaultId",
                principalTable: "FridgeFaults",
                principalColumn: "FridgeFaultId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedFaults_FaultReports_FaultReportId",
                table: "AssignedFaults");

            migrationBuilder.DropForeignKey(
                name: "FK_FaultReports_CustomerFridges_CustomerFridgeId",
                table: "FaultReports");

            migrationBuilder.DropForeignKey(
                name: "FK_FaultReports_FridgeFaults_FridgeFaultId",
                table: "FaultReports");

            migrationBuilder.DropIndex(
                name: "IX_FaultReports_CustomerFridgeId",
                table: "FaultReports");

            migrationBuilder.DropIndex(
                name: "IX_FaultReports_FridgeFaultId",
                table: "FaultReports");

            migrationBuilder.DropIndex(
                name: "IX_AssignedFaults_FaultReportId",
                table: "AssignedFaults");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "FridgeRequests");

            migrationBuilder.DropColumn(
                name: "CustomerFridgeId",
                table: "FaultReports");

            migrationBuilder.DropColumn(
                name: "FridgeFaultId",
                table: "FaultReports");

            migrationBuilder.DropColumn(
                name: "FaultReportId",
                table: "AssignedFaults");
        }
    }
}
