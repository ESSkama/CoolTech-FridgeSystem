using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class fault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersNotifications_FaultReports_FaultReportFaultId",
                table: "CustomersNotifications");

            migrationBuilder.AlterColumn<int>(
                name: "FaultReportFaultId",
                table: "CustomersNotifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FaultId",
                table: "CustomersNotifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersNotifications_FaultReports_FaultReportFaultId",
                table: "CustomersNotifications",
                column: "FaultReportFaultId",
                principalTable: "FaultReports",
                principalColumn: "FaultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersNotifications_FaultReports_FaultReportFaultId",
                table: "CustomersNotifications");

            migrationBuilder.AlterColumn<int>(
                name: "FaultReportFaultId",
                table: "CustomersNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FaultId",
                table: "CustomersNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersNotifications_FaultReports_FaultReportFaultId",
                table: "CustomersNotifications",
                column: "FaultReportFaultId",
                principalTable: "FaultReports",
                principalColumn: "FaultId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
