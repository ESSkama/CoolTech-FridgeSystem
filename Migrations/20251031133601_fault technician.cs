using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class faulttechnician : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedFaults_FaultTechnicians_FaultTechId",
                table: "AssignedFaults");

            migrationBuilder.DropIndex(
                name: "IX_AssignedFaults_FaultTechId",
                table: "AssignedFaults");

            migrationBuilder.AddColumn<int>(
                name: "FaultTechnicianFaultTechId",
                table: "AssignedFaults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TechnicianId",
                table: "AssignedFaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AssignedFaults_FaultTechnicianFaultTechId",
                table: "AssignedFaults",
                column: "FaultTechnicianFaultTechId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedFaults_TechnicianId",
                table: "AssignedFaults",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedFaults_Employees_TechnicianId",
                table: "AssignedFaults",
                column: "TechnicianId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedFaults_FaultTechnicians_FaultTechnicianFaultTechId",
                table: "AssignedFaults",
                column: "FaultTechnicianFaultTechId",
                principalTable: "FaultTechnicians",
                principalColumn: "FaultTechId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedFaults_Employees_TechnicianId",
                table: "AssignedFaults");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedFaults_FaultTechnicians_FaultTechnicianFaultTechId",
                table: "AssignedFaults");

            migrationBuilder.DropIndex(
                name: "IX_AssignedFaults_FaultTechnicianFaultTechId",
                table: "AssignedFaults");

            migrationBuilder.DropIndex(
                name: "IX_AssignedFaults_TechnicianId",
                table: "AssignedFaults");

            migrationBuilder.DropColumn(
                name: "FaultTechnicianFaultTechId",
                table: "AssignedFaults");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "AssignedFaults");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedFaults_FaultTechId",
                table: "AssignedFaults",
                column: "FaultTechId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedFaults_FaultTechnicians_FaultTechId",
                table: "AssignedFaults",
                column: "FaultTechId",
                principalTable: "FaultTechnicians",
                principalColumn: "FaultTechId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
