using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public virtual DbSet<AuditLog> AuditLogs { get; set; }

        public virtual DbSet<ConsumptionRecord> ConsumptionRecords { get; set; }

        public virtual DbSet<ExceptionEvent> ExceptionEvents { get; set; }

        public virtual DbSet<Facility> Facilities { get; set; }

        public virtual DbSet<Forecast> Forecasts { get; set; }

        public virtual DbSet<InventoryPosition> InventoryPositions { get; set; }

        public virtual DbSet<Item> Items { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        public virtual DbSet<RecallAction> RecallActions { get; set; }

        public virtual DbSet<Receipt> Receipts { get; set; }

        public virtual DbSet<ReplenishmentPlan> ReplenishmentPlans { get; set; }

        public virtual DbSet<SensorDevice> SensorDevices { get; set; }

        public virtual DbSet<StorageZone> StorageZones { get; set; }

        public virtual DbSet<Supplier> Suppliers { get; set; }

        public virtual DbSet<TelemetryRecord> TelemetryRecords { get; set; }

        public virtual DbSet<TransferOrder> TransferOrders { get; set; }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F23B8C3E074D9");

                entity.ToTable("AuditLog");

                entity.Property(e => e.AuditId).HasColumnName("AuditID");
                entity.Property(e => e.Timestamp)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__AuditLog__UserID__4CA06362");
            });

            modelBuilder.Entity<ConsumptionRecord>(entity =>
            {
                entity.HasKey(e => e.ConsumptionId).HasName("PK__Consumpt__E3A1C4378B9665DA");

                entity.ToTable("ConsumptionRecord");

                entity.Property(e => e.ConsumptionId).HasColumnName("ConsumptionID");
                entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
                entity.Property(e => e.ItemId).HasColumnName("ItemID");
                entity.Property(e => e.QuantityUsed).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Timestamp)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.WardId)
                    .HasMaxLength(50)
                    .HasColumnName("WardID");

                entity.HasOne(d => d.Facility).WithMany(p => p.ConsumptionRecords)
                    .HasForeignKey(d => d.FacilityId)
                    .HasConstraintName("FK__Consumpti__Facil__6B24EA82");

                entity.HasOne(d => d.Item).WithMany(p => p.ConsumptionRecords)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK__Consumpti__ItemI__6C190EBB");

                entity.HasOne(d => d.UsedByNavigation).WithMany(p => p.ConsumptionRecords)
                    .HasForeignKey(d => d.UsedBy)
                    .HasConstraintName("FK__Consumpti__UsedB__6D0D32F4");
            });

            modelBuilder.Entity<ExceptionEvent>(entity =>
            {
                entity.HasKey(e => e.ExceptionId).HasName("PK__Exceptio__26981DA8A7E92A3C");

                entity.ToTable("ExceptionEvent");

                entity.Property(e => e.ExceptionId).HasColumnName("ExceptionID");
                entity.Property(e => e.DetectedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
                entity.Property(e => e.Severity).HasMaxLength(20);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<Facility>(entity =>
            {
                entity.HasKey(e => e.FacilityId).HasName("PK__Facility__5FB08B94A6B7EFA8");

                entity.ToTable("Facility");

                entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Region).HasMaxLength(100);
                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<Forecast>(entity =>
            {
                entity.HasKey(e => e.ForecastId).HasName("PK__Forecast__7F274458D83E5BDD");

                entity.ToTable("Forecast");

                entity.Property(e => e.ForecastId).HasColumnName("ForecastID");
                entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
                entity.Property(e => e.ForecastQuantity).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.GeneratedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ItemId).HasColumnName("ItemID");
                entity.Property(e => e.Period).HasMaxLength(50);

                entity.HasOne(d => d.Facility).WithMany(p => p.Forecasts)
                    .HasForeignKey(d => d.FacilityId)
                    .HasConstraintName("FK__Forecast__Facili__7E37BEF6");

                entity.HasOne(d => d.Item).WithMany(p => p.Forecasts)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK__Forecast__ItemID__7D439ABD");
            });

            modelBuilder.Entity<InventoryPosition>(entity =>
            {
                entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__F5FDE6D3526CA965");

                entity.ToTable("InventoryPosition");

                entity.Property(e => e.InventoryId).HasColumnName("InventoryID");
                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
                entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
                entity.Property(e => e.ItemId).HasColumnName("ItemID");
                entity.Property(e => e.LotId)
                    .HasMaxLength(50)
                    .HasColumnName("LotID");
                entity.Property(e => e.QuantityOnHand).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.SafetyStock).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ZoneId).HasColumnName("ZoneID");

                entity.HasOne(d => d.Facility).WithMany(p => p.InventoryPositions)
                    .HasForeignKey(d => d.FacilityId)
                    .HasConstraintName("FK__Inventory__Facil__619B8048");

                entity.HasOne(d => d.Item).WithMany(p => p.InventoryPositions)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK__Inventory__ItemI__6383C8BA");

                entity.HasOne(d => d.Zone).WithMany(p => p.InventoryPositions)
                    .HasForeignKey(d => d.ZoneId)
                    .HasConstraintName("FK__Inventory__ZoneI__628FA481");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.ItemId).HasName("PK__Item__727E83EB77C36BB0");

                entity.ToTable("Item");

                entity.Property(e => e.ItemId).HasColumnName("ItemID");
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.ItemName).HasMaxLength(255);
                entity.Property(e => e.UnitOfMeasure).HasMaxLength(20);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E329F078352");

                entity.ToTable("Notification");

                entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Notificat__UserI__05D8E0BE");
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.HasKey(e => e.Poid).HasName("PK__Purchase__5F02A2F49D60AFC8");

                entity.ToTable("PurchaseOrder");

                entity.Property(e => e.Poid).HasColumnName("POID");
                entity.Property(e => e.ExpectedDeliveryDate).HasColumnType("datetime");
                entity.Property(e => e.OrderDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrders)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK__PurchaseO__Suppl__59063A47");
            });

            modelBuilder.Entity<RecallAction>(entity =>
            {
                entity.HasKey(e => e.RecallId).HasName("PK__RecallAc__DB2339A37A8DED28");

                entity.ToTable("RecallAction");

                entity.Property(e => e.RecallId).HasColumnName("RecallID");
                entity.Property(e => e.DueDate).HasColumnType("datetime");
                entity.Property(e => e.ExceptionId).HasColumnName("ExceptionID");
                entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(d => d.Exception).WithMany(p => p.RecallActions)
                    .HasForeignKey(d => d.ExceptionId)
                    .HasConstraintName("FK__RecallAct__Excep__797309D9");

                entity.HasOne(d => d.Owner).WithMany(p => p.RecallActions)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK__RecallAct__Owner__7A672E12");
            });

            modelBuilder.Entity<Receipt>(entity =>
            {
                entity.HasKey(e => e.ReceiptId).HasName("PK__Receipt__CC08C4002C0CB92B");

                entity.ToTable("Receipt");

                entity.Property(e => e.ReceiptId).HasColumnName("ReceiptID");
                entity.Property(e => e.Poid).HasColumnName("POID");
                entity.Property(e => e.QualityStatus).HasMaxLength(50);
                entity.Property(e => e.ReceivedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.SupplierLot).HasMaxLength(50);

                entity.HasOne(d => d.Po).WithMany(p => p.Receipts)
                    .HasForeignKey(d => d.Poid)
                    .HasConstraintName("FK__Receipt__POID__5CD6CB2B");

                entity.HasOne(d => d.ReceivedByNavigation).WithMany(p => p.Receipts)
                    .HasForeignKey(d => d.ReceivedBy)
                    .HasConstraintName("FK__Receipt__Receive__5EBF139D");
            });

            modelBuilder.Entity<ReplenishmentPlan>(entity =>
            {
                entity.HasKey(e => e.PlanId).HasName("PK__Replenis__755C22D79E99CF29");

                entity.ToTable("ReplenishmentPlan");

                entity.Property(e => e.PlanId).HasColumnName("PlanID");
                entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
                entity.Property(e => e.ItemId).HasColumnName("ItemID");
                entity.Property(e => e.Priority).HasMaxLength(20);
                entity.Property(e => e.SuggestedOrderQty).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Facility).WithMany(p => p.ReplenishmentPlans)
                    .HasForeignKey(d => d.FacilityId)
                    .HasConstraintName("FK__Replenish__Facil__02FC7413");

                entity.HasOne(d => d.Item).WithMany(p => p.ReplenishmentPlans)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK__Replenish__ItemI__02084FDA");
            });

            modelBuilder.Entity<SensorDevice>(entity =>
            {
                entity.HasKey(e => e.SensorId).HasName("PK__SensorDe__D809841AE627EC96");

                entity.ToTable("SensorDevice");

                entity.Property(e => e.SensorId).HasColumnName("SensorID");
                entity.Property(e => e.AssignedTo).HasMaxLength(100);
                entity.Property(e => e.DeviceType).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
            });

            modelBuilder.Entity<StorageZone>(entity =>
            {
                entity.HasKey(e => e.ZoneId).HasName("PK__StorageZ__601667959C48E490");

                entity.ToTable("StorageZone");

                entity.Property(e => e.ZoneId).HasColumnName("ZoneID");
                entity.Property(e => e.Capacity).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.TemperatureProfile).HasMaxLength(50);

                entity.HasOne(d => d.Facility).WithMany(p => p.StorageZones)
                    .HasForeignKey(d => d.FacilityId)
                    .HasConstraintName("FK__StorageZo__Facil__52593CB8");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE6669486F1630F");

                entity.ToTable("Supplier");

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.SupplierType).HasMaxLength(50);
            });

            modelBuilder.Entity<TelemetryRecord>(entity =>
            {
                entity.HasKey(e => e.TelemetryId).HasName("PK__Telemetr__157CAF1744C4D9A2");

                entity.ToTable("TelemetryRecord");

                entity.Property(e => e.TelemetryId).HasColumnName("TelemetryID");
                entity.Property(e => e.Humidity).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.Location).HasMaxLength(255);
                entity.Property(e => e.SensorId).HasColumnName("SensorID");
                entity.Property(e => e.Temperature).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.Timestamp)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Sensor).WithMany(p => p.TelemetryRecords)
                    .HasForeignKey(d => d.SensorId)
                    .HasConstraintName("FK__Telemetry__Senso__72C60C4A");
            });

            modelBuilder.Entity<TransferOrder>(entity =>
            {
                entity.HasKey(e => e.TransferId).HasName("PK__Transfer__954901719145A1B0");

                entity.ToTable("TransferOrder");

                entity.Property(e => e.TransferId).HasColumnName("TransferID");
                entity.Property(e => e.FromFacilityId).HasColumnName("FromFacilityID");
                entity.Property(e => e.ItemId).HasColumnName("ItemID");
                entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.ToFacilityId).HasColumnName("ToFacilityID");

                entity.HasOne(d => d.FromFacility).WithMany(p => p.TransferOrderFromFacilities)
                    .HasForeignKey(d => d.FromFacilityId)
                    .HasConstraintName("FK__TransferO__FromF__66603565");

                entity.HasOne(d => d.Item).WithMany(p => p.TransferOrders)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK__TransferO__ItemI__68487DD7");

                entity.HasOne(d => d.ToFacility).WithMany(p => p.TransferOrderToFacilities)
                    .HasForeignKey(d => d.ToFacilityId)
                    .HasConstraintName("FK__TransferO__ToFac__6754599E");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC8A5CE041");

                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__A9D1053401F659CA").IsUnique();

                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasDefaultValue("");
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Role).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
