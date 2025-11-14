using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeSystem.Migrations
{
    /// <inheritdoc />
    public partial class upfatedmerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerLocations_Customers_CustomerId",
                table: "CustomerLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_FaultTechnicians_Warehouses_WarehouseId",
                table: "FaultTechnicians");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Suppliers",
                newName: "Telephone");

            migrationBuilder.RenameColumn(
                name: "FridgeImageURL",
                table: "Fridges",
                newName: "FridgeImage");

            migrationBuilder.RenameColumn(
                name: "DelivereyDate",
                table: "Fridges",
                newName: "DeliveryDate");

            migrationBuilder.RenameColumn(
                name: "LocationName",
                table: "CustomerLocations",
                newName: "BranchName");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "CustomerLocations",
                newName: "BusinessId");

            migrationBuilder.RenameColumn(
                name: "ContactPhone",
                table: "CustomerLocations",
                newName: "Telephone");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerLocations_CustomerId",
                table: "CustomerLocations",
                newName: "IX_CustomerLocations_BusinessId");

            migrationBuilder.AddColumn<int>(
                name: "CustomerLocationId",
                table: "Fridges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FaultReportFaultId",
                table: "Fridges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "Fridges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "FridgeRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CustomerLocationId",
                table: "FridgeRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "FridgeRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "FridgeRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewedById",
                table: "FridgeRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CustomerLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    HiredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Event = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "EmployeesNotifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesNotifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_EmployeesNotifications_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FridgesInventory",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuantityInStock = table.Column<int>(type: "int", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    ManagedById = table.Column<int>(type: "int", nullable: false),
                    InventoryLiasonEmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FridgesInventory", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_FridgesInventory_Employees_InventoryLiasonEmployeeId",
                        column: x => x.InventoryLiasonEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FridgesInventory_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceChecklists",
                columns: table => new
                {
                    ChecklistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChecklists", x => x.ChecklistId);
                    table.ForeignKey(
                        name: "FK_ServiceChecklists_Employees_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaultReports",
                columns: table => new
                {
                    FaultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PriorityLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BusinessId = table.Column<int>(type: "int", nullable: false),
                    CustomerBusinessId = table.Column<int>(type: "int", nullable: false),
                    InventoryId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    AssignedById = table.Column<int>(type: "int", nullable: true),
                    AdminEmployeeId = table.Column<int>(type: "int", nullable: false),
                    AssignedToId = table.Column<int>(type: "int", nullable: true),
                    RequestStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaultReports", x => x.FaultId);
                    table.ForeignKey(
                        name: "FK_FaultReports_CustomerLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaultReports_Customers_CustomerBusinessId",
                        column: x => x.CustomerBusinessId,
                        principalTable: "Customers",
                        principalColumn: "BusinessId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaultReports_Employees_AdminEmployeeId",
                        column: x => x.AdminEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaultReports_Employees_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FaultReports_FridgesInventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "FridgesInventory",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FridgesAllocations",
                columns: table => new
                {
                    AllocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllocatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InventoryId = table.Column<int>(type: "int", nullable: false),
                    FridgeId = table.Column<int>(type: "int", nullable: false),
                    BusinessId = table.Column<int>(type: "int", nullable: false),
                    BusinessNameBusinessId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    RequestStatus = table.Column<int>(type: "int", nullable: false),
                    AllocatedById = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FridgesAllocations", x => x.AllocationId);
                    table.ForeignKey(
                        name: "FK_FridgesAllocations_CustomerLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FridgesAllocations_Customers_BusinessNameBusinessId",
                        column: x => x.BusinessNameBusinessId,
                        principalTable: "Customers",
                        principalColumn: "BusinessId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FridgesAllocations_Employees_AllocatedById",
                        column: x => x.AllocatedById,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FridgesAllocations_FridgeRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "FridgeRequests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FridgesAllocations_FridgesInventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "FridgesInventory",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FridgesAllocations_Fridges_FridgeId",
                        column: x => x.FridgeId,
                        principalTable: "Fridges",
                        principalColumn: "FridgeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FridgeStatusHistory",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    InventoryId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    FridgeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FridgeStatusHistory", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_FridgeStatusHistory_CustomerLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FridgeStatusHistory_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FridgeStatusHistory_EventTypes_EventId",
                        column: x => x.EventId,
                        principalTable: "EventTypes",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FridgeStatusHistory_FridgesInventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "FridgesInventory",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FridgeStatusHistory_Fridges_FridgeId",
                        column: x => x.FridgeId,
                        principalTable: "Fridges",
                        principalColumn: "FridgeId");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantityRequested = table.Column<int>(type: "int", nullable: false),
                    ReasonForRequest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedById = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: false),
                    InventoryId = table.Column<int>(type: "int", nullable: false),
                    RequestStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_Employees_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_FridgesInventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "FridgesInventory",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "serviceCheckItems",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SequenceOrder = table.Column<int>(type: "int", nullable: false),
                    ChecklistId = table.Column<int>(type: "int", nullable: false),
                    ServiceChecklistChecklistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serviceCheckItems", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_serviceCheckItems_ServiceChecklists_ServiceChecklistChecklistId",
                        column: x => x.ServiceChecklistChecklistId,
                        principalTable: "ServiceChecklists",
                        principalColumn: "ChecklistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSchedules",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FridgeId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    FaultId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSchedules", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_ServiceSchedules_CustomerLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceSchedules_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceSchedules_EventTypes_EventId",
                        column: x => x.EventId,
                        principalTable: "EventTypes",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceSchedules_FaultReports_FaultId",
                        column: x => x.FaultId,
                        principalTable: "FaultReports",
                        principalColumn: "FaultId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceSchedules_Fridges_FridgeId",
                        column: x => x.FridgeId,
                        principalTable: "Fridges",
                        principalColumn: "FridgeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomersNotifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BusinessId = table.Column<int>(type: "int", nullable: false),
                    FaultId = table.Column<int>(type: "int", nullable: false),
                    FaultReportFaultId = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomersNotifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_CustomersNotifications_Customers_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Customers",
                        principalColumn: "BusinessId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomersNotifications_FaultReports_FaultReportFaultId",
                        column: x => x.FaultReportFaultId,
                        principalTable: "FaultReports",
                        principalColumn: "FaultId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomersNotifications_ServiceSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "ServiceSchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FaultDiagnostics",
                columns: table => new
                {
                    DiagnosisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiagnosisDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FaultId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    RequestStatus = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaultDiagnostics", x => x.DiagnosisId);
                    table.ForeignKey(
                        name: "FK_FaultDiagnostics_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FaultDiagnostics_FaultReports_FaultId",
                        column: x => x.FaultId,
                        principalTable: "FaultReports",
                        principalColumn: "FaultId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FaultDiagnostics_ServiceSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "ServiceSchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAssignments",
                columns: table => new
                {
                    AssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FaultId = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    AssignedById = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAssignments", x => x.AssignmentId);
                    table.ForeignKey(
                        name: "FK_ServiceAssignments_Employees_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceAssignments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceAssignments_FaultReports_FaultId",
                        column: x => x.FaultId,
                        principalTable: "FaultReports",
                        principalColumn: "FaultId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceAssignments_ServiceSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "ServiceSchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCheckResults",
                columns: table => new
                {
                    ResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CheckedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    RequestStatus = table.Column<int>(type: "int", nullable: false),
                    CheckedById = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCheckResults", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_ServiceCheckResults_Employees_CheckedById",
                        column: x => x.CheckedById,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCheckResults_ServiceSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "ServiceSchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceCheckResults_serviceCheckItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "serviceCheckItems",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fridges_CustomerLocationId",
                table: "Fridges",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Fridges_FaultReportFaultId",
                table: "Fridges",
                column: "FaultReportFaultId");

            migrationBuilder.CreateIndex(
                name: "IX_Fridges_InventoryId",
                table: "Fridges",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgeRequests_CustomerLocationId",
                table: "FridgeRequests",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgeRequests_InventoryId",
                table: "FridgeRequests",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgeRequests_ReviewedById",
                table: "FridgeRequests",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmployeeId",
                table: "AspNetUsers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersNotifications_BusinessId",
                table: "CustomersNotifications",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersNotifications_FaultReportFaultId",
                table: "CustomersNotifications",
                column: "FaultReportFaultId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersNotifications_ScheduleId",
                table: "CustomersNotifications",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesNotifications_EmployeeId",
                table: "EmployeesNotifications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultDiagnostics_EmployeeId",
                table: "FaultDiagnostics",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultDiagnostics_FaultId",
                table: "FaultDiagnostics",
                column: "FaultId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultDiagnostics_ScheduleId",
                table: "FaultDiagnostics",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultReports_AdminEmployeeId",
                table: "FaultReports",
                column: "AdminEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultReports_AssignedToId",
                table: "FaultReports",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultReports_CustomerBusinessId",
                table: "FaultReports",
                column: "CustomerBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultReports_InventoryId",
                table: "FaultReports",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FaultReports_LocationId",
                table: "FaultReports",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgesAllocations_AllocatedById",
                table: "FridgesAllocations",
                column: "AllocatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FridgesAllocations_BusinessNameBusinessId",
                table: "FridgesAllocations",
                column: "BusinessNameBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgesAllocations_FridgeId",
                table: "FridgesAllocations",
                column: "FridgeId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgesAllocations_InventoryId",
                table: "FridgesAllocations",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgesAllocations_LocationId",
                table: "FridgesAllocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgesAllocations_RequestId",
                table: "FridgesAllocations",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgesInventory_InventoryLiasonEmployeeId",
                table: "FridgesInventory",
                column: "InventoryLiasonEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgesInventory_SupplierId",
                table: "FridgesInventory",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgeStatusHistory_EmployeeId",
                table: "FridgeStatusHistory",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgeStatusHistory_EventId",
                table: "FridgeStatusHistory",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgeStatusHistory_FridgeId",
                table: "FridgeStatusHistory",
                column: "FridgeId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgeStatusHistory_InventoryId",
                table: "FridgeStatusHistory",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FridgeStatusHistory_LocationId",
                table: "FridgeStatusHistory",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_InventoryId",
                table: "PurchaseRequests",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_ManagerId",
                table: "PurchaseRequests",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_RequestedById",
                table: "PurchaseRequests",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAssignments_AssignedById",
                table: "ServiceAssignments",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAssignments_EmployeeId",
                table: "ServiceAssignments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAssignments_FaultId",
                table: "ServiceAssignments",
                column: "FaultId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAssignments_ScheduleId",
                table: "ServiceAssignments",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_serviceCheckItems_ServiceChecklistChecklistId",
                table: "serviceCheckItems",
                column: "ServiceChecklistChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChecklists_CreatedById",
                table: "ServiceChecklists",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCheckResults_CheckedById",
                table: "ServiceCheckResults",
                column: "CheckedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCheckResults_ItemId",
                table: "ServiceCheckResults",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCheckResults_ScheduleId",
                table: "ServiceCheckResults",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_EmployeeId",
                table: "ServiceSchedules",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_EventId",
                table: "ServiceSchedules",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_FaultId",
                table: "ServiceSchedules",
                column: "FaultId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_FridgeId",
                table: "ServiceSchedules",
                column: "FridgeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_LocationId",
                table: "ServiceSchedules",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Employees_EmployeeId",
                table: "AspNetUsers",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerLocations_Customers_BusinessId",
                table: "CustomerLocations",
                column: "BusinessId",
                principalTable: "Customers",
                principalColumn: "BusinessId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaultTechnicians_Warehouses_WarehouseId",
                table: "FaultTechnicians",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "WarehouseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FridgeRequests_CustomerLocations_CustomerLocationId",
                table: "FridgeRequests",
                column: "CustomerLocationId",
                principalTable: "CustomerLocations",
                principalColumn: "CustomerLocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FridgeRequests_Employees_ReviewedById",
                table: "FridgeRequests",
                column: "ReviewedById",
                principalTable: "Employees",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FridgeRequests_FridgesInventory_InventoryId",
                table: "FridgeRequests",
                column: "InventoryId",
                principalTable: "FridgesInventory",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fridges_CustomerLocations_CustomerLocationId",
                table: "Fridges",
                column: "CustomerLocationId",
                principalTable: "CustomerLocations",
                principalColumn: "CustomerLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fridges_FaultReports_FaultReportFaultId",
                table: "Fridges",
                column: "FaultReportFaultId",
                principalTable: "FaultReports",
                principalColumn: "FaultId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fridges_FridgesInventory_InventoryId",
                table: "Fridges",
                column: "InventoryId",
                principalTable: "FridgesInventory",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Employees_EmployeeId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerLocations_Customers_BusinessId",
                table: "CustomerLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_FaultTechnicians_Warehouses_WarehouseId",
                table: "FaultTechnicians");

            migrationBuilder.DropForeignKey(
                name: "FK_FridgeRequests_CustomerLocations_CustomerLocationId",
                table: "FridgeRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FridgeRequests_Employees_ReviewedById",
                table: "FridgeRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FridgeRequests_FridgesInventory_InventoryId",
                table: "FridgeRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Fridges_CustomerLocations_CustomerLocationId",
                table: "Fridges");

            migrationBuilder.DropForeignKey(
                name: "FK_Fridges_FaultReports_FaultReportFaultId",
                table: "Fridges");

            migrationBuilder.DropForeignKey(
                name: "FK_Fridges_FridgesInventory_InventoryId",
                table: "Fridges");

            migrationBuilder.DropTable(
                name: "CustomersNotifications");

            migrationBuilder.DropTable(
                name: "EmployeesNotifications");

            migrationBuilder.DropTable(
                name: "FaultDiagnostics");

            migrationBuilder.DropTable(
                name: "FridgesAllocations");

            migrationBuilder.DropTable(
                name: "FridgeStatusHistory");

            migrationBuilder.DropTable(
                name: "PurchaseRequests");

            migrationBuilder.DropTable(
                name: "ServiceAssignments");

            migrationBuilder.DropTable(
                name: "ServiceCheckResults");

            migrationBuilder.DropTable(
                name: "ServiceSchedules");

            migrationBuilder.DropTable(
                name: "serviceCheckItems");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "FaultReports");

            migrationBuilder.DropTable(
                name: "ServiceChecklists");

            migrationBuilder.DropTable(
                name: "FridgesInventory");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Fridges_CustomerLocationId",
                table: "Fridges");

            migrationBuilder.DropIndex(
                name: "IX_Fridges_FaultReportFaultId",
                table: "Fridges");

            migrationBuilder.DropIndex(
                name: "IX_Fridges_InventoryId",
                table: "Fridges");

            migrationBuilder.DropIndex(
                name: "IX_FridgeRequests_CustomerLocationId",
                table: "FridgeRequests");

            migrationBuilder.DropIndex(
                name: "IX_FridgeRequests_InventoryId",
                table: "FridgeRequests");

            migrationBuilder.DropIndex(
                name: "IX_FridgeRequests_ReviewedById",
                table: "FridgeRequests");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmployeeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CustomerLocationId",
                table: "Fridges");

            migrationBuilder.DropColumn(
                name: "FaultReportFaultId",
                table: "Fridges");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "Fridges");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "FridgeRequests");

            migrationBuilder.DropColumn(
                name: "CustomerLocationId",
                table: "FridgeRequests");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "FridgeRequests");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "FridgeRequests");

            migrationBuilder.DropColumn(
                name: "ReviewedById",
                table: "FridgeRequests");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CustomerLocations");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Telephone",
                table: "Suppliers",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "FridgeImage",
                table: "Fridges",
                newName: "FridgeImageURL");

            migrationBuilder.RenameColumn(
                name: "DeliveryDate",
                table: "Fridges",
                newName: "DelivereyDate");

            migrationBuilder.RenameColumn(
                name: "Telephone",
                table: "CustomerLocations",
                newName: "ContactPhone");

            migrationBuilder.RenameColumn(
                name: "BusinessId",
                table: "CustomerLocations",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "BranchName",
                table: "CustomerLocations",
                newName: "LocationName");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerLocations_BusinessId",
                table: "CustomerLocations",
                newName: "IX_CustomerLocations_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerLocations_Customers_CustomerId",
                table: "CustomerLocations",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "BusinessId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaultTechnicians_Warehouses_WarehouseId",
                table: "FaultTechnicians",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "WarehouseId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
