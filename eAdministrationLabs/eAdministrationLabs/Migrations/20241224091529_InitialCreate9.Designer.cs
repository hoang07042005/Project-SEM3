﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eAdministrationLabs.Models;

#nullable disable

namespace eAdministrationLabs.Migrations
{
    [DbContext(typeof(EAdministrationLabsContext))]
    [Migration("20241224091529_InitialCreate9")]
    partial class InitialCreate9
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("eAdministrationLabs.Models.EquiLab", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("EquiLabID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EquipmentId")
                        .HasColumnType("int")
                        .HasColumnName("EquipmentID");

                    b.Property<int>("LabId")
                        .HasColumnType("int")
                        .HasColumnName("LabID");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__EquiLabs__80AC09910675DFE1");

                    b.HasIndex("EquipmentId");

                    b.HasIndex("LabId");

                    b.HasIndex("UserId");

                    b.ToTable("EquiLabs");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Equipment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("EquipmentID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("NameEquipment")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("PurchaseDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id")
                        .HasName("PK__Equipmen__3447459958A3481B");

                    b.ToTable("Equipments");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Feedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("FeedBackID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Comment")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)")
                        .HasColumnName("Comment");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("Quality")
                        .HasColumnType("int")
                        .HasColumnName("Quality");

                    b.Property<int>("Rating")
                        .HasColumnType("int")
                        .HasColumnName("Rating");

                    b.Property<int>("RequestId")
                        .HasColumnType("int")
                        .HasColumnName("RequestID");

                    b.Property<int>("Satisfaction")
                        .HasColumnType("int")
                        .HasColumnName("Satisfaction");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("UpdatedAt")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("Id")
                        .HasName("PK__Feedback__3447459958A3481B");

                    b.HasIndex("RequestId");

                    b.HasIndex("UserId");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.HistoryRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("HistoryID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ChangedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("ChangedBy")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RequestId")
                        .HasColumnType("int")
                        .HasColumnName("RequestID");

                    b.Property<int>("StatusRequestId")
                        .HasColumnType("int")
                        .HasColumnName("StatusRequestID");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("Id")
                        .HasName("PK__HistoryR__4D7B4ADD8A14BF35");

                    b.HasIndex("RequestId");

                    b.HasIndex("StatusRequestId");

                    b.HasIndex("UserId");

                    b.ToTable("HistoryRequests");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Lab", b =>
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

                    b.Property<int>("StatusLabId")
                        .HasColumnType("int")
                        .HasColumnName("StatusLabID");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("Id")
                        .HasName("PK__Labs__EDBD773A77FA477F");

                    b.HasIndex("StatusLabId");

                    b.ToTable("Labs");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.LabUsageLog", b =>
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

                    b.Property<int>("LabId")
                        .HasColumnType("int")
                        .HasColumnName("LabID");

                    b.Property<string>("Purpose")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("Id")
                        .HasName("PK__LabUsage__5E5499A8EB6A0186");

                    b.HasIndex("LabId");

                    b.HasIndex("UserId");

                    b.ToTable("LabUsageLogs");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Notification", b =>
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

                    b.Property<int?>("LabUsageLogId")
                        .HasColumnType("int")
                        .HasColumnName("LabUsageLogID");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReadStatus")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Unread");

                    b.Property<int?>("RequestId")
                        .HasColumnType("int")
                        .HasColumnName("RequestID");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("Id")
                        .HasName("PK__Notifica__20CF2E3234B214C8");

                    b.HasIndex("LabUsageLogId");

                    b.HasIndex("RequestId");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RequestID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("EquipmentId")
                        .HasColumnType("int")
                        .HasColumnName("EquipmentID");

                    b.Property<int>("ImageId")
                        .HasColumnType("int")
                        .HasColumnName("ImageID");

                    b.Property<int>("LabId")
                        .HasColumnType("int")
                        .HasColumnName("LabID");

                    b.HasKey("Id")
                        .HasName("PK__Requests__33A8519A00162311");

                    b.HasIndex("EquipmentId");

                    b.HasIndex("ImageId");

                    b.HasIndex("LabId");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.RequestImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ImageID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id")
                        .HasName("PK__RequestI__7516F4EC4DE8D026");

                    b.ToTable("RequestImages");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Role", b =>
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
                        .HasName("PK__Roles__8AFACE3A4754D5DB");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Software", b =>
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

                    b.Property<int>("LabId")
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
                        .HasName("PK__Software__25EDB8DCBF9069AC");

                    b.HasIndex("LabId");

                    b.ToTable("Softwares");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.StatusLab", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("StatusLabID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("StatusName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id")
                        .HasName("PK__StatusLa__072A22DFB420707C");

                    b.ToTable("StatusLabs");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.StatusRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("StatusRequestID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("StatusName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id")
                        .HasName("PK__StatusRe__B62FB50E95575741");

                    b.ToTable("StatusRequests");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.User", b =>
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

                    b.Property<string>("PasswordResetToken")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("ResetToken");

                    b.Property<DateTime?>("TokenExpirationTime")
                        .HasColumnType("datetime")
                        .HasColumnName("TokenExpiry");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("Id")
                        .HasName("PK__Users__1788CCAC10AD3EDE");

                    b.HasIndex(new[] { "Email" }, "UQ__Users__A9D105344E93A6A9")
                        .IsUnique();

                    b.ToTable("Users", t =>
                        {
                            t.HasTrigger("trg_Users_UpdatedAt");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("eAdministrationLabs.Models.UserRole", b =>
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
                        .HasName("PK__UserRole__3D978A55B7812925");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.EquiLab", b =>
                {
                    b.HasOne("eAdministrationLabs.Models.Equipment", "Equipment")
                        .WithMany("EquiLabs")
                        .HasForeignKey("EquipmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__EquiLabs__Equipm__4BAC3F29");

                    b.HasOne("eAdministrationLabs.Models.Lab", "Lab")
                        .WithMany("EquiLabs")
                        .HasForeignKey("LabId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__EquiLabs__LabID__4CA06362");

                    b.HasOne("eAdministrationLabs.Models.User", null)
                        .WithMany("EquiLabs")
                        .HasForeignKey("UserId");

                    b.Navigation("Equipment");

                    b.Navigation("Lab");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Feedback", b =>
                {
                    b.HasOne("eAdministrationLabs.Models.HistoryRequest", "HistoryRequest")
                        .WithMany("Feedbacks")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("eAdministrationLabs.Models.User", "User")
                        .WithMany("Feedbacks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("HistoryRequest");

                    b.Navigation("User");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.HistoryRequest", b =>
                {
                    b.HasOne("eAdministrationLabs.Models.Request", "Request")
                        .WithMany("HistoryRequests")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__HistoryRe__Reque__68487DD7");

                    b.HasOne("eAdministrationLabs.Models.StatusRequest", "StatusRequest")
                        .WithMany("HistoryRequests")
                        .HasForeignKey("StatusRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__HistoryRe__Statu__693CA210");

                    b.HasOne("eAdministrationLabs.Models.User", "User")
                        .WithMany("HistoryRequests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__HistoryRe__UserI__6754599E");

                    b.Navigation("Request");

                    b.Navigation("StatusRequest");

                    b.Navigation("User");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Lab", b =>
                {
                    b.HasOne("eAdministrationLabs.Models.StatusLab", "StatusLab")
                        .WithMany("Labs")
                        .HasForeignKey("StatusLabId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Labs__StatusLabI__45F365D3");

                    b.Navigation("StatusLab");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.LabUsageLog", b =>
                {
                    b.HasOne("eAdministrationLabs.Models.Lab", "Lab")
                        .WithMany("LabUsageLogs")
                        .HasForeignKey("LabId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__LabUsageL__LabID__5812160E");

                    b.HasOne("eAdministrationLabs.Models.User", "User")
                        .WithMany("LabUsageLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__LabUsageL__UserI__59063A47");

                    b.Navigation("Lab");

                    b.Navigation("User");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Notification", b =>
                {
                    b.HasOne("eAdministrationLabs.Models.LabUsageLog", "LabUsageLog")
                        .WithMany()
                        .HasForeignKey("LabUsageLogId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("eAdministrationLabs.Models.Request", "Request")
                        .WithMany()
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK__Notification__RequestId__F9F9A3A7");

                    b.HasOne("eAdministrationLabs.Models.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Notificat__UserI__6EF57B66");

                    b.Navigation("LabUsageLog");

                    b.Navigation("Request");

                    b.Navigation("User");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Request", b =>
                {
                    b.HasOne("eAdministrationLabs.Models.Equipment", "Equipment")
                        .WithMany("Requests")
                        .HasForeignKey("EquipmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK__Requests__Equipm__628FA481");

                    b.HasOne("eAdministrationLabs.Models.RequestImage", "Image")
                        .WithMany("Requests")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Requests__ImageI__6383C8BA");

                    b.HasOne("eAdministrationLabs.Models.Lab", "Lab")
                        .WithMany("Requests")
                        .HasForeignKey("LabId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Requests__LabID__619B8048");

                    b.Navigation("Equipment");

                    b.Navigation("Image");

                    b.Navigation("Lab");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Software", b =>
                {
                    b.HasOne("eAdministrationLabs.Models.Lab", "Lab")
                        .WithMany("Softwares")
                        .HasForeignKey("LabId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__Softwares__LabID__5441852A");

                    b.Navigation("Lab");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.UserRole", b =>
                {
                    b.HasOne("eAdministrationLabs.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__UserRoles__RoleI__412EB0B6");

                    b.HasOne("eAdministrationLabs.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__UserRoles__UserI__403A8C7D");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Equipment", b =>
                {
                    b.Navigation("EquiLabs");

                    b.Navigation("Requests");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.HistoryRequest", b =>
                {
                    b.Navigation("Feedbacks");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Lab", b =>
                {
                    b.Navigation("EquiLabs");

                    b.Navigation("LabUsageLogs");

                    b.Navigation("Requests");

                    b.Navigation("Softwares");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Request", b =>
                {
                    b.Navigation("HistoryRequests");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.RequestImage", b =>
                {
                    b.Navigation("Requests");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.Role", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.StatusLab", b =>
                {
                    b.Navigation("Labs");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.StatusRequest", b =>
                {
                    b.Navigation("HistoryRequests");
                });

            modelBuilder.Entity("eAdministrationLabs.Models.User", b =>
                {
                    b.Navigation("EquiLabs");

                    b.Navigation("Feedbacks");

                    b.Navigation("HistoryRequests");

                    b.Navigation("LabUsageLogs");

                    b.Navigation("Notifications");

                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
