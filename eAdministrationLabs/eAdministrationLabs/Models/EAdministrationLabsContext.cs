using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace eAdministrationLabs.Models;

public partial class EAdministrationLabsContext : DbContext
{
    public EAdministrationLabsContext()
    {
    }

    public EAdministrationLabsContext(DbContextOptions<EAdministrationLabsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EquiLab> EquiLabs { get; set; }

    public virtual DbSet<Equipment> Equipments { get; set; }

    public virtual DbSet<HistoryRequest> HistoryRequests { get; set; }

    public virtual DbSet<Lab> Labs { get; set; }

    public virtual DbSet<LabUsageLog> LabUsageLogs { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestImage> RequestImages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Software> Softwares { get; set; }

    public virtual DbSet<StatusLab> StatusLabs { get; set; }

    public virtual DbSet<StatusRequest> StatusRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=DESKTOP-MT3BHB0\\SQLEXPRESS01;Database=eAdministrationLabs;Trusted_Connection=True;trustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EquiLab>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EquiLabs__80AC09911AF6FCC0");

            entity.Property(e => e.Id).HasColumnName("EquiLabID");
            entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");
            entity.Property(e => e.LabId).HasColumnName("LabID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Equipment).WithMany(p => p.EquiLabs)
                .HasForeignKey(d => d.EquipmentId)
                .HasConstraintName("FK__EquiLabs__Equipm__4BAC3F29");

            entity.HasOne(d => d.Lab).WithMany(p => p.EquiLabs)
                .HasForeignKey(d => d.LabId)
                .HasConstraintName("FK__EquiLabs__LabID__4CA06362");

            entity.HasOne(d => d.User).WithMany(p => p.EquiLabs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__EquiLabs__UserID__4D94879B");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Equipmen__34474599C099BD7A");

            entity.Property(e => e.Id).HasColumnName("EquipmentID");
            entity.Property(e => e.NameEquipment).HasMaxLength(50);
            entity.Property(e => e.PurchaseDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<HistoryRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HistoryR__4D7B4ADD9C22A41E");

            entity.Property(e => e.Id).HasColumnName("HistoryID");
            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ChangedBy).HasMaxLength(100);
            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.StatusRequestId).HasColumnName("StatusRequestID");

            entity.HasOne(d => d.Request).WithMany(p => p.HistoryRequests)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK__HistoryRe__Reque__68487DD7");

            entity.HasOne(d => d.StatusRequest).WithMany(p => p.HistoryRequests)
                .HasForeignKey(d => d.StatusRequestId)
                .HasConstraintName("FK__HistoryRe__Statu__693CA210");
        });

        modelBuilder.Entity<Lab>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Labs__EDBD773AFD35145D");

            entity.Property(e => e.Id).HasColumnName("LabID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LabName).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.StatusLabId).HasColumnName("StatusLabID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.StatusLab).WithMany(p => p.Labs)
                .HasForeignKey(d => d.StatusLabId)
                .HasConstraintName("FK__Labs__StatusLabI__45F365D3");
        });

        modelBuilder.Entity<LabUsageLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LabUsage__5E5499A891CDA483");

            entity.Property(e => e.Id).HasColumnName("LogID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.LabId).HasColumnName("LabID");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Lab).WithMany(p => p.LabUsageLogs)
                .HasForeignKey(d => d.LabId)
                .HasConstraintName("FK__LabUsageL__LabID__5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.LabUsageLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__LabUsageL__UserI__59063A47");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__20CF2E327F781F3A");

            entity.Property(e => e.Id).HasColumnName("NotificationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReadStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Unread");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__6EF57B66");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Requests__33A8519A2D96C229");

            entity.Property(e => e.Id).HasColumnName("RequestID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");
            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.LabId).HasColumnName("LabID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Equipment).WithMany(p => p.Requests)
                .HasForeignKey(d => d.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Requests__Equipm__6383C8BA");

            entity.HasOne(d => d.Image).WithMany(p => p.Requests)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("FK__Requests__ImageI__6477ECF3");

            entity.HasOne(d => d.Lab).WithMany(p => p.Requests)
                .HasForeignKey(d => d.LabId)
                .HasConstraintName("FK__Requests__LabID__628FA481");

            entity.HasOne(d => d.User).WithMany(p => p.Requests)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Requests__UserID__619B8048");
        });

        modelBuilder.Entity<RequestImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RequestI__7516F4EC9B4E4FB6");

            entity.Property(e => e.Id).HasColumnName("ImageID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__8AFACE3A12AC46C2");

            entity.Property(e => e.Id).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Software>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Software__25EDB8DC784E20C8");

            entity.Property(e => e.Id).HasColumnName("SoftwareID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LabId).HasColumnName("LabID");
            entity.Property(e => e.License)
                .HasMaxLength(50)
                .HasDefaultValue("Free");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Version).HasMaxLength(50);

            entity.HasOne(d => d.Lab).WithMany(p => p.Softwares)
                .HasForeignKey(d => d.LabId)
                .HasConstraintName("FK__Softwares__LabID__5441852A");
        });

        modelBuilder.Entity<StatusLab>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StatusLa__072A22DFBE90E538");

            entity.Property(e => e.Id).HasColumnName("StatusLabID");
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<StatusRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StatusRe__B62FB50E9DBB1A7F");

            entity.Property(e => e.Id).HasColumnName("StatusRequestID");
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__1788CCACD9780C3D");

            entity.ToTable(tb => tb.HasTrigger("trg_Users_UpdatedAt"));

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053471AAC870").IsUnique();

            entity.Property(e => e.Id).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3D978A55F8AEFCEB");

            entity.Property(e => e.Id).HasColumnName("UserRoleID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__UserRoles__RoleI__412EB0B6");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserRoles__UserI__403A8C7D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
