using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class reportnull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedFaults_FaultReports_FaultReportId",
                table: "AssignedFaults");

            migrationBuilder.AlterColumn<int>(
                name: "FaultReportId",
                table: "AssignedFaults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedFaults_FaultReports_FaultReportId",
                table: "AssignedFaults",
                column: "FaultReportId",
                principalTable: "FaultReports",
                principalColumn: "FaultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedFaults_FaultReports_FaultReportId",
                table: "AssignedFaults");

            migrationBuilder.AlterColumn<int>(
                name: "FaultReportId",
                table: "AssignedFaults",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedFaults_FaultReports_FaultReportId",
                table: "AssignedFaults",
                column: "FaultReportId",
                principalTable: "FaultReports",
                principalColumn: "FaultId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
