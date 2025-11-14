using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class www : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FridgeFaultId",
                table: "ServiceSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ServiceSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_FridgeFaultId",
                table: "ServiceSchedules",
                column: "FridgeFaultId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceSchedules_FridgeFaults_FridgeFaultId",
                table: "ServiceSchedules",
                column: "FridgeFaultId",
                principalTable: "FridgeFaults",
                principalColumn: "FridgeFaultId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceSchedules_FridgeFaults_FridgeFaultId",
                table: "ServiceSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ServiceSchedules_FridgeFaultId",
                table: "ServiceSchedules");

            migrationBuilder.DropColumn(
                name: "FridgeFaultId",
                table: "ServiceSchedules");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ServiceSchedules");
        }
    }
}
