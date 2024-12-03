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

    public virtual DbSet<Computer> Computers { get; set; }

    public virtual DbSet<Lab> Labs { get; set; }

    public virtual DbSet<LabUsageLog> LabUsageLogs { get; set; }

    public virtual DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Software> Softwares { get; set; }

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
        modelBuilder.Entity<Computer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Computer__A6BE3C544C7E7DFA");

            entity.HasIndex(e => e.AssetTag, "UQ__Computer__89F276AB1E7B772C").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ComputerID");
            entity.Property(e => e.AssetTag).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LabId).HasColumnName("LabID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Working");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Lab).WithMany(p => p.Computers)
                .HasForeignKey(d => d.LabId)
                .HasConstraintName("FK__Computers__LabID__48CFD27E");
        });

        modelBuilder.Entity<Lab>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Labs__EDBD773AC13EA201");

            entity.Property(e => e.Id).HasColumnName("LabID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LabName).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Available");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<LabUsageLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LabUsage__5E5499A8EA71F6EC");

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
                .HasConstraintName("FK__LabUsageL__LabID__5629CD9C");

            entity.HasOne(d => d.User).WithMany(p => p.LabUsageLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__LabUsageL__UserI__571DF1D5");
        });

        modelBuilder.Entity<MaintenanceRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Maintena__33A8519AC47A51B5");

            entity.Property(e => e.Id).HasColumnName("RequestID");
            entity.Property(e => e.ComputerId).HasColumnName("ComputerID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LabId).HasColumnName("LabID");
            entity.Property(e => e.ResolvedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Computer).WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(d => d.ComputerId)
                .HasConstraintName("FK__Maintenan__Compu__5BE2A6F2");

            entity.HasOne(d => d.Lab).WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(d => d.LabId)
                .HasConstraintName("FK__Maintenan__LabID__5AEE82B9");

            entity.HasOne(d => d.RequestedByNavigation).WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(d => d.RequestedBy)
                .HasConstraintName("FK__Maintenan__Reque__5EBF139D");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__20CF2E326425BAE3");

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
                .HasConstraintName("FK__Notificat__UserI__628FA481");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__8AFACE3A9E01B887");

            entity.Property(e => e.Id).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Software>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Software__25EDB8DC178A2AE4");

            entity.ToTable("Software");

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
                .HasConstraintName("FK__Software__LabID__5165187F");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__1788CCAC7038AFC9");

            entity.ToTable(tb => tb.HasTrigger("trg_Users_UpdatedAt"));

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053449081518").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3D978A550CD4DA5F");

            entity.Property(e => e.Id).HasColumnName("UserRoleID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__RoleI__3F466844");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__UserI__3E52440B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
