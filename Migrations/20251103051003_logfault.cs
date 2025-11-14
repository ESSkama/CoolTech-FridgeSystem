using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class logfault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FaultStatus",
                table: "FridgeFaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SelectedCategory",
                table: "FridgeFaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TechnicianId",
                table: "FridgeFaults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "FridgeFaults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Title",
                table: "FaultReports",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "TechnicianId",
                table: "AssignedFaults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FaultTechId",
                table: "AssignedFaults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "FridgeImage",
                table: "AssignedFaults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FridgeFaults_TechnicianId",
                table: "FridgeFaults",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_FridgeFaults_Employees_TechnicianId",
                table: "FridgeFaults",
                column: "TechnicianId",
                principalTable: "Employees",
                principalColumn: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FridgeFaults_Employees_TechnicianId",
                table: "FridgeFaults");

            migrationBuilder.DropIndex(
                name: "IX_FridgeFaults_TechnicianId",
                table: "FridgeFaults");

            migrationBuilder.DropColumn(
                name: "FaultStatus",
                table: "FridgeFaults");

            migrationBuilder.DropColumn(
                name: "SelectedCategory",
                table: "FridgeFaults");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "FridgeFaults");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "FridgeFaults");

            migrationBuilder.DropColumn(
                name: "FridgeImage",
                table: "AssignedFaults");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "FaultReports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TechnicianId",
                table: "AssignedFaults",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FaultTechId",
                table: "AssignedFaults",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
