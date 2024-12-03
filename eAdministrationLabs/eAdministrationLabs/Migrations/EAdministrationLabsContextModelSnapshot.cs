﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eAdministrationLabs.Models;

#nullable disable

namespace eAdministrationLabs.Migrations
{
    [DbContext(typeof(EAdministrationLabsContext))]
    partial class EAdministrationLabsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Test.Models.Computer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ComputerID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AssetTag")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("LabId")
                        .HasColumnType("int")
                        .HasColumnName("LabID");

                    b.Property<string>("Specifications")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Working");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("Id")
                        .HasName("PK__Computer__A6BE3C544C7E7DFA");

                    b.HasIndex("LabId");

                    b.HasIndex(new[] { "AssetTag" }, "UQ__Computer__89F276AB1E7B772C")
                        .IsUnique();

                    b.ToTable("Computers");
                });

            modelBuilder.Entity("Test.Models.Lab", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("LabID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("LabName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Status")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Available");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("Id")
                        .HasName("PK__Labs__EDBD773AC13EA201");

                    b.ToTable("Labs");
                });

            modelBuilder.Entity("Test.Models.LabUsageLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("LogID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime");

                    b.Property<int?>("LabId")
                        .HasColumnType("int")
                        .HasColumnName("LabID");

                    b.Property<string>("Purpose")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("Id")
                        .HasName("PK__LabUsage__5E5499A8EA71F6EC");

                    b.HasIndex("LabId");

                    b.HasIndex("UserId");

                    b.ToTable("LabUsageLogs");
                });

            modelBuilder.Entity("Test.Models.MaintenanceRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RequestID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ComputerId")
                        .HasColumnType("int")
                        .HasColumnName("ComputerID");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("LabId")
                        .HasColumnType("int")
                        .HasColumnName("LabID");

                    b.Property<int?>("RequestedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ResolvedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Status")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Pending");

                    b.HasKey("Id")
                        .HasName("PK__Maintena__33A8519AC47A51B5");

                    b.HasIndex("ComputerId");

                    b.HasIndex("LabId");

                    b.HasIndex("RequestedBy");

                    b.ToTable("MaintenanceRequests");
                });

            modelBuilder.Entity("Test.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("NotificationID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReadStatus")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Unread");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("Id")
                        .HasName("PK__Notifica__20CF2E326425BAE3");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Test.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RoleID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id")
                        .HasName("PK__Roles__8AFACE3A9E01B887");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Test.Models.Software", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("SoftwareID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("LabId")
                        .HasColumnType("int")
                        .HasColumnName("LabID");

                    b.Property<string>("License")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Free");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id")
                        .HasName("PK__Software__25EDB8DC178A2AE4");

                    b.HasIndex("LabId");

                    b.ToTable("Software", (string)null);
                });

            modelBuilder.Entity("Test.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("Id")
                        .HasName("PK__Users__1788CCAC7038AFC9");

                    b.HasIndex(new[] { "Email" }, "UQ__Users__A9D1053449081518")
                        .IsUnique();

                    b.ToTable("Users", t =>
                        {
                            t.HasTrigger("trg_Users_UpdatedAt");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("Test.Models.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UserRoleID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RoleID");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("Id")
                        .HasName("PK__UserRole__3D978A550CD4DA5F");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Test.Models.Computer", b =>
                {
                    b.HasOne("Test.Models.Lab", "Lab")
                        .WithMany("Computers")
                        .HasForeignKey("LabId")
                        .HasConstraintName("FK__Computers__LabID__48CFD27E");

                    b.Navigation("Lab");
                });

            modelBuilder.Entity("Test.Models.LabUsageLog", b =>
                {
                    b.HasOne("Test.Models.Lab", "Lab")
                        .WithMany("LabUsageLogs")
                        .HasForeignKey("LabId")
                        .HasConstraintName("FK__LabUsageL__LabID__5629CD9C");

                    b.HasOne("Test.Models.User", "User")
                        .WithMany("LabUsageLogs")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__LabUsageL__UserI__571DF1D5");

                    b.Navigation("Lab");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Test.Models.MaintenanceRequest", b =>
                {
                    b.HasOne("Test.Models.Computer", "Computer")
                        .WithMany("MaintenanceRequests")
                        .HasForeignKey("ComputerId")
                        .HasConstraintName("FK__Maintenan__Compu__5BE2A6F2");

                    b.HasOne("Test.Models.Lab", "Lab")
                        .WithMany("MaintenanceRequests")
                        .HasForeignKey("LabId")
                        .HasConstraintName("FK__Maintenan__LabID__5AEE82B9");

                    b.HasOne("Test.Models.User", "RequestedByNavigation")
                        .WithMany("MaintenanceRequests")
                        .HasForeignKey("RequestedBy")
                        .HasConstraintName("FK__Maintenan__Reque__5EBF139D");

                    b.Navigation("Computer");

                    b.Navigation("Lab");

                    b.Navigation("RequestedByNavigation");
                });

            modelBuilder.Entity("Test.Models.Notification", b =>
                {
                    b.HasOne("Test.Models.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Notificat__UserI__628FA481");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Test.Models.Software", b =>
                {
                    b.HasOne("Test.Models.Lab", "Lab")
                        .WithMany("Softwares")
                        .HasForeignKey("LabId")
                        .HasConstraintName("FK__Software__LabID__5165187F");

                    b.Navigation("Lab");
                });

            modelBuilder.Entity("Test.Models.UserRole", b =>
                {
                    b.HasOne("Test.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .IsRequired()
                        .HasConstraintName("FK__UserRoles__RoleI__3F466844");

                    b.HasOne("Test.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK__UserRoles__UserI__3E52440B");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Test.Models.Computer", b =>
                {
                    b.Navigation("MaintenanceRequests");
                });

            modelBuilder.Entity("Test.Models.Lab", b =>
                {
                    b.Navigation("Computers");

                    b.Navigation("LabUsageLogs");

                    b.Navigation("MaintenanceRequests");

                    b.Navigation("Softwares");
                });

            modelBuilder.Entity("Test.Models.Role", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("Test.Models.User", b =>
                {
                    b.Navigation("LabUsageLogs");

                    b.Navigation("MaintenanceRequests");

                    b.Navigation("Notifications");

                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}