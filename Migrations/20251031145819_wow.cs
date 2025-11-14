using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class wow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "FridgeRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "FridgeRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BusinessName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MaintenanceSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FridgeId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeWindow = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaintenanceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Technician = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceSchedules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FridgeRequests_ApplicationUserId",
                table: "FridgeRequests",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgeRequests_CustomerId",
                table: "FridgeRequests",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FridgeRequests_AspNetUsers_ApplicationUserId",
                table: "FridgeRequests",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FridgeRequests_AspNetUsers_CustomerId",
                table: "FridgeRequests",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FridgeRequests_AspNetUsers_ApplicationUserId",
                table: "FridgeRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FridgeRequests_AspNetUsers_CustomerId",
                table: "FridgeRequests");

            migrationBuilder.DropTable(
                name: "MaintenanceSchedules");

            migrationBuilder.DropIndex(
                name: "IX_FridgeRequests_ApplicationUserId",
                table: "FridgeRequests");

            migrationBuilder.DropIndex(
                name: "IX_FridgeRequests_CustomerId",
                table: "FridgeRequests");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "FridgeRequests");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "FridgeRequests");

            migrationBuilder.DropColumn(
                name: "BusinessName",
                table: "AspNetUsers");
        }
    }
}
