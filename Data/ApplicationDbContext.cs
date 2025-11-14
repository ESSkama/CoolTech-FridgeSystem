using FridgeSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FridgeSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Fridge> Fridges { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<AssignedFault> AssignedFaults { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerFridge> CustomerFridges { get; set; }
        public DbSet<CustomerLocation> CustomerLocations { get; set; }
        public DbSet<FaultTechnician> FaultTechnicians { get; set; }
        public DbSet<FridgeFault> FridgeFaults { get; set; }
        public DbSet<FridgeRequest> FridgeRequests { get; set; }
        public DbSet<FridgeRequestItem> FridgeRequestItems { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseStock> WarehouseStocks { get; set; }

        public DbSet<CustomerNotification> CustomersNotifications { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeNotification> EmployeesNotifications { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<FaultDiagnostic> FaultDiagnostics { get; set; }
        public DbSet<FaultReport> FaultReports { get; set; }
        public DbSet<FridgeAllocation> FridgesAllocations { get; set; }
        public DbSet<FridgeInventory> FridgesInventory { get; set; }
        public DbSet<FridgeStatusHistory> FridgeStatusHistory { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<ServiceAssignment> ServiceAssignments { get; set; }
        public DbSet<ServiceCheckItem> serviceCheckItems { get; set; }
        public DbSet<ServiceChecklist> ServiceChecklists { get; set; }
        public DbSet<ServiceCheckResult> ServiceCheckResults { get; set; }
        public DbSet<ServiceSchedule> ServiceSchedules { get; set; }
        public DbSet<BusinessType> BusinessTypes { get; set; }
        public DbSet<MaintenanceVisit> MaintenanceVisit { get; set; }
        public DbSet<ServiceHistory> ServiceHistory { get; set; }
        public DbSet<MaintenanceSchedule> MaintenanceSchedules { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Keep Identity setup

            // ================= APPLICATION USER LINKS =================
            modelBuilder.Entity<FaultTechnician>()
                .HasOne(ft => ft.ApplicationUser)
                .WithOne(au => au.Technician)
                .HasForeignKey<FaultTechnician>(ft => ft.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.ApplicationUser)
                .WithOne()
                .HasForeignKey<Customer>(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= CUSTOMER RELATIONSHIPS =================
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.CustomerLocations)
                .WithOne(cl => cl.Customer)
                .HasForeignKey(cl => cl.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1️⃣ Relationship between ApplicationUser and FridgeRequest
            modelBuilder.Entity<ApplicationUser>()
                .HasMany<FridgeRequest>()
                .WithOne(fr => fr.Customer)
                .HasForeignKey(fr => fr.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2️⃣ Relationship between Customer (business) and FridgeRequest
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.FridgeRequests)
                .WithOne(fr => fr.Business)
                .HasForeignKey(fr => fr.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);


            // Configure the many-to-many relationship
            modelBuilder.Entity<FridgeRequestItem>()
                .HasKey(fri => fri.FridgeRequestItemId); // Assuming Id is primary key for join table

            modelBuilder.Entity<FridgeRequestItem>()
                .HasOne(fri => fri.FridgeRequest)
                .WithMany(fr => fr.FridgeRequestItems)
                .HasForeignKey(fri => fri.RequestId);

            modelBuilder.Entity<FridgeRequestItem>()
                .HasOne(fri => fri.Fridge)
                .WithMany() // A fridge can be in many requests, but no direct collection on Fridge model for simplicity
                .HasForeignKey(fri => fri.FridgeId);



            modelBuilder.Entity<CustomerNotification>(entity =>
            {
                entity.HasKey(n => n.NotificationId);

                entity.HasOne(n => n.Customer)
                      .WithMany(c => c.Notifications)
                      .HasForeignKey(n => n.BusinessId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Schedule)
                      .WithMany(ss => ss.Notifications)
                      .HasForeignKey(n => n.ScheduleId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ================= CUSTOMER LOCATION LINKS =================
            modelBuilder.Entity<CustomerLocation>()
                .HasMany(cl => cl.CustomerFridges)
                .WithOne(cf => cf.CustomerLocation)
                .HasForeignKey(cf => cf.CustomerLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerLocation>()
                .HasMany(cl => cl.FridgeRequests)
                .WithOne(fr => fr.CustomerLocation)
                .HasForeignKey(fr => fr.CustomerLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= FRIDGE REQUESTS =================
            modelBuilder.Entity<FridgeRequest>()
                .HasOne(fr => fr.Warehouse)
                .WithMany()
                .HasForeignKey(fr => fr.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FridgeRequest>()
                .HasMany(fr => fr.FridgeRequestItems)
                .WithOne(fri => fri.FridgeRequest)
                .HasForeignKey(fri => fri.RequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FridgeRequest>()
                .HasMany(fr => fr.FridgeAllocations)
                .WithOne(fa => fa.FridgeRequest)
                .HasForeignKey(fa => fa.RequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FridgeRequestItem>()
                .HasOne(fri => fri.Fridge)
                .WithMany(f => f.FridgeRequestItems)
                .HasForeignKey(fri => fri.FridgeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= FRIDGES & INVENTORY =================
            modelBuilder.Entity<Fridge>()
                .HasOne(f => f.Supplier)
                .WithMany(s => s.SuppliedFridges)
                .HasForeignKey(f => f.SupplierId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fridge>()
                .HasMany(f => f.ServiceSchedules)
                .WithOne(ss => ss.Fridge)
                .HasForeignKey(ss => ss.FridgeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FridgeInventory>()
                .HasMany(fi => fi.Fridges)
                .WithOne(f => f.Inventory)
                .HasForeignKey(f => f.InventoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FridgeInventory>()
                .HasMany(fi => fi.FridgeAllocations)
                .WithOne(fa => fa.FridgeInventory)
                .HasForeignKey(fa => fa.InventoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FridgeInventory>()
                .HasMany(fi => fi.FaultReports)
                .WithOne(fr => fr.FridgeInventory)
                .HasForeignKey(fr => fr.InventoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= FRIDGE ALLOCATIONS =================
            modelBuilder.Entity<FridgeAllocation>()
                .HasOne(fa => fa.Fridge)
                .WithMany(f => f.FridgeAllocations)
                .HasForeignKey(fa => fa.FridgeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FridgeAllocation>()
                .HasOne(fa => fa.CustomerLocation)
                .WithMany(cl => cl.FridgeAllocations)
                .HasForeignKey(fa => fa.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            

            modelBuilder.Entity<FridgeAllocation>()
                .HasOne(fa => fa.CustomerLiaison)
                .WithMany(e => e.Allocations)
                .HasForeignKey(fa => fa.AllocatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= FAULT MANAGEMENT =================
            modelBuilder.Entity<FridgeFault>()
                .HasOne(ff => ff.CustomerFridge)
                .WithMany(cf => cf.FridgeFaults)
                .HasForeignKey(ff => ff.CustomerFridgeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FridgeFault>()
    .HasMany(f => f.ServiceSchedules)
    .WithOne(s => s.FridgeFault)
    .HasForeignKey(s => s.FridgeFaultId)  // ✅ The FK is in ServiceSchedule
    .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<AssignedFault>()
                .HasOne(af => af.FridgeFault)
                .WithMany(ff => ff.AssignedFaults)
                .HasForeignKey(af => af.FridgeFaultId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignedFault>()
                .HasOne(af => af.Technician)
                .WithMany(ft => ft.AssignedFaults)
                .HasForeignKey(af => af.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= EMPLOYEES & NOTIFICATIONS =================
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Notifications)
                .WithOne(en => en.Employee)
                .HasForeignKey(en => en.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.AllocatedFaults)
                .WithOne(f => f.FaultTechnician)
                .HasForeignKey(f => f.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.AssignedServices)
                .WithOne(sa => sa.Technician)
                .HasForeignKey(sa => sa.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.PurchaseRequests)
                .WithOne(pr => pr.RequestedBy)
                .HasForeignKey(pr => pr.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.PurchaseRequestsSubmitted)
                .WithOne(pr => pr.Employee)
                .HasForeignKey(pr => pr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= SERVICE & SCHEDULES =================
            modelBuilder.Entity<ServiceSchedule>(entity =>
            {
                entity.HasKey(ss => ss.ScheduleId);

                entity.HasOne(ss => ss.Fridge)
                      .WithMany(f => f.ServiceSchedules)
                      .HasForeignKey(ss => ss.FridgeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ss => ss.CustomerLocation)
                      .WithMany(l => l.ServiceSchedules)
                      .HasForeignKey(ss => ss.LocationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ss => ss.FaultReport)
                      .WithMany(fr => fr.Schedules)
                      .HasForeignKey(ss => ss.FaultId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ss => ss.ServiceType)
                      .WithMany(et => et.ServiceSchedules)
                      .HasForeignKey(ss => ss.EventId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ss => ss.Employee)
                      .WithMany(e => e.ServiceSchedules)
                      .HasForeignKey(ss => ss.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ================= FRIDGE STATUS HISTORY =================
            modelBuilder.Entity<FridgeStatusHistory>(entity =>
            {
                entity.HasKey(fsh => fsh.HistoryId);

                entity.HasOne(fsh => fsh.EventType)
                      .WithMany(et => et.FridgeStatusHistory)
                      .HasForeignKey(fsh => fsh.EventId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(fsh => fsh.FridgeInventory)
                      .WithMany(fi => fi.FridgeStatusHistory)
                      .HasForeignKey(fsh => fsh.InventoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(fsh => fsh.Location)
                      .WithMany(l => l.FridgeStatusHistory)
                      .HasForeignKey(fsh => fsh.LocationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(fsh => fsh.Employee)
                      .WithMany(e => e.FridgeStatusHistory)
                      .HasForeignKey(fsh => fsh.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);


            });

            // ================= UNIQUE CONSTRAINTS =================
            modelBuilder.Entity<CustomerFridge>()
                .HasIndex(cf => cf.SerialNumber)
                .IsUnique();

            modelBuilder.Entity<WarehouseStock>()
                .HasIndex(ws => new { ws.WarehouseId, ws.FridgeId })
                .IsUnique();

            // ================= WAREHOUSE STOCK =================
            modelBuilder.Entity<WarehouseStock>()
                .HasOne(ws => ws.Warehouse)
                .WithMany(w => w.WarehouseStocks)
                .HasForeignKey(ws => ws.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WarehouseStock>()
                .HasOne(ws => ws.Fridge)
                .WithMany(f => f.WarehouseStocks)
                .HasForeignKey(ws => ws.FridgeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CustomerFridge>()
        .HasOne(cf => cf.Fridge)
        .WithMany(f => f.CustomerFridges)
        .HasForeignKey(cf => cf.FridgeId)
        .OnDelete(DeleteBehavior.Restrict);

            // CustomerFridge ↔ CustomerLocation
            modelBuilder.Entity<CustomerFridge>()
                .HasOne(cf => cf.CustomerLocation)
                .WithMany(cl => cl.CustomerFridges)
                .HasForeignKey(cf => cf.CustomerLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= FAULT DIAGNOSTICS =================
            modelBuilder.Entity<FaultDiagnostic>()
                .HasOne(fd => fd.FaultReport)
                .WithMany(fr => fr.Diagnostics) // Assuming FaultReport has a collection of FaultDiagnostics
                .HasForeignKey(fd => fd.FaultId)
                .OnDelete(DeleteBehavior.Restrict); // Explicitly set to Restrict

            modelBuilder.Entity<FaultDiagnostic>()
        .HasOne(fd => fd.Employee) // Assuming FaultDiagnostic has an Employee navigation property
        .WithMany(e => e.Diagnostics) // Assuming Employee has a collection of FaultDiagnostics
        .HasForeignKey(fd => fd.EmployeeId)
        .OnDelete(DeleteBehavior.Restrict);


            // Same for ServiceSchedule if needed
            modelBuilder.Entity<FaultDiagnostic>()
                .HasOne(fd => fd.Schedule) // Assuming FaultDiagnostic has a ServiceSchedule navigation property
                .WithMany(ss => ss.FaultDiagnostics) // Assuming ServiceSchedule has a collection of FaultDiagnostics
                .HasForeignKey(fd => fd.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);




            // ================= SERVICE ASSIGNMENTS =================
// Add this new configuration block
            modelBuilder.Entity<ServiceAssignment>()
    .HasOne(sa => sa.FaultReport) // ServiceAssignment has one FaultReport
    .WithMany(fr => fr.ServiceAssignments) // Assuming FaultReport has a collection of ServiceAssignments
    .HasForeignKey(sa => sa.FaultId) // Use the specific FK column from your CREATE TABLE
    .OnDelete(DeleteBehavior.Restrict); // <--- THIS IS THE CRUCIAL CHANGE

            // Consider other ON DELETE CASCADE relationships on ServiceAssignments if they could cause issues:
            // FK_ServiceAssignments_ServiceSchedules_ScheduleId (also ON DELETE CASCADE)
            // If deleting a ServiceSchedule should *not* cascade delete its ServiceAssignments, change it too.
            modelBuilder.Entity<ServiceAssignment>()
                .HasOne(sa => sa.Schedule)
                .WithMany(ss => ss.ServiceAssignments) // Assuming ServiceSchedule has a collection of ServiceAssignments
                .HasForeignKey(sa => sa.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict); // Consider changing this as well if deleting a schedule shouldn't delete assignments

            // For the Employees relationships, one is NO ACTION already, the other is CASCADE.
            // FK_ServiceAssignments_Employees_AssignedById (ON DELETE CASCADE)
            // If an Employee is deleted, should all their assignments as 'AssignedBy' be deleted?
            // Often, you'd want to restrict this or reassign.
            modelBuilder.Entity<ServiceAssignment>()
                .HasOne(sa => sa.AssignedBy) // Assuming a navigation property for who assigned it
                .WithMany(e => e.ServicesAssigned) // Example navigation property name
                .HasForeignKey(sa => sa.AssignedById)
                .OnDelete(DeleteBehavior.Restrict); // Consider changing this to restrict

            // FK_ServiceAssignments_Employees_EmployeeId (ON DELETE NO ACTION) - This one is already good.
           modelBuilder.Entity<ServiceAssignment>()
                .HasOne(sa => sa.Technician) // You already have this in your initial code block
                .WithMany(e => e.AssignedServices)
                .HasForeignKey(sa => sa.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict); // This was already NO ACTION in the SQL, which translates to Restrict.

            // ... existing configurations

            // ================= SERVICE CHECK RESULTS =================
            // Add this new configuration block
            modelBuilder.Entity<ServiceCheckResult>()
                .HasOne(scr => scr.Item) // ServiceCheckResult has one ServiceCheckItem
                .WithMany(sci => sci.ServiceCheckResults) // Assuming ServiceCheckItem has a collection of ServiceCheckResults
                .HasForeignKey(scr => scr.ItemId) // Use the correct foreign key property
                .OnDelete(DeleteBehavior.Restrict); // <--- THIS IS THE CRUCIAL CHANGE

            // Also consider other ON DELETE CASCADE relationships on ServiceCheckResults.
            // FK_ServiceCheckResults_ServiceSchedules_ServiceScheduleScheduleId (ON DELETE CASCADE)
            // If deleting a ServiceSchedule should *not* cascade delete its ServiceCheckResults, change this too.
            modelBuilder.Entity<ServiceCheckResult>()
                .HasOne(scr => scr.ServiceSchedule) // Assuming navigation property
                .WithMany(ss => ss.ServiceCheckResults) // Assuming ServiceSchedule has a collection of ServiceCheckResults
                .HasForeignKey(scr => scr.ScheduleId) // The specific FK name in SQL
                .OnDelete(DeleteBehavior.Restrict); // Consider changing this to restrict

            // FK_ServiceCheckResults_Employees_CheckedById (ON DELETE CASCADE)
            // If an Employee is deleted, should all their check results be deleted?
            // Often, you'd want to restrict this or reassign/keep historical data.
            modelBuilder.Entity<ServiceCheckResult>()
                .HasOne(scr => scr.CheckedBy) // Assuming a navigation property for who checked it
                .WithMany(e => e.ServiceCheckresults) // Example navigation property name
                .HasForeignKey(scr => scr.CheckedById)
                .OnDelete(DeleteBehavior.Restrict); // Consider changing this to restrict

            // ... rest of your OnModelCreating

       




        }
    }  }

