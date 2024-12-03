using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAdministrationLabs.Migrations
{
    /// <inheritdoc />
    public partial class CreateData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Labs",
                columns: table => new
                {
                    LabID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Available"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Labs__EDBD773AC13EA201", x => x.LabID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__8AFACE3A9E01B887", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CCAC7038AFC9", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Computers",
                columns: table => new
                {
                    ComputerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabID = table.Column<int>(type: "int", nullable: true),
                    AssetTag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Working"),
                    Specifications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Computer__A6BE3C544C7E7DFA", x => x.ComputerID);
                    table.ForeignKey(
                        name: "FK__Computers__LabID__48CFD27E",
                        column: x => x.LabID,
                        principalTable: "Labs",
                        principalColumn: "LabID");
                });

            migrationBuilder.CreateTable(
                name: "Software",
                columns: table => new
                {
                    SoftwareID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    License = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Free"),
                    LabID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Software__25EDB8DC178A2AE4", x => x.SoftwareID);
                    table.ForeignKey(
                        name: "FK__Software__LabID__5165187F",
                        column: x => x.LabID,
                        principalTable: "Labs",
                        principalColumn: "LabID");
                });

            migrationBuilder.CreateTable(
                name: "LabUsageLogs",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LabUsage__5E5499A8EA71F6EC", x => x.LogID);
                    table.ForeignKey(
                        name: "FK__LabUsageL__LabID__5629CD9C",
                        column: x => x.LabID,
                        principalTable: "Labs",
                        principalColumn: "LabID");
                    table.ForeignKey(
                        name: "FK__LabUsageL__UserI__571DF1D5",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Unread"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__20CF2E326425BAE3", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK__Notificat__UserI__628FA481",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserRoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserRole__3D978A550CD4DA5F", x => x.UserRoleID);
                    table.ForeignKey(
                        name: "FK__UserRoles__RoleI__3F466844",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID");
                    table.ForeignKey(
                        name: "FK__UserRoles__UserI__3E52440B",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceRequests",
                columns: table => new
                {
                    RequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabID = table.Column<int>(type: "int", nullable: true),
                    ComputerID = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Pending"),
                    RequestedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ResolvedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Maintena__33A8519AC47A51B5", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK__Maintenan__Compu__5BE2A6F2",
                        column: x => x.ComputerID,
                        principalTable: "Computers",
                        principalColumn: "ComputerID");
                    table.ForeignKey(
                        name: "FK__Maintenan__LabID__5AEE82B9",
                        column: x => x.LabID,
                        principalTable: "Labs",
                        principalColumn: "LabID");
                    table.ForeignKey(
                        name: "FK__Maintenan__Reque__5EBF139D",
                        column: x => x.RequestedBy,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Computers_LabID",
                table: "Computers",
                column: "LabID");

            migrationBuilder.CreateIndex(
                name: "UQ__Computer__89F276AB1E7B772C",
                table: "Computers",
                column: "AssetTag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabUsageLogs_LabID",
                table: "LabUsageLogs",
                column: "LabID");

            migrationBuilder.CreateIndex(
                name: "IX_LabUsageLogs_UserID",
                table: "LabUsageLogs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_ComputerID",
                table: "MaintenanceRequests",
                column: "ComputerID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_LabID",
                table: "MaintenanceRequests",
                column: "LabID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_RequestedBy",
                table: "MaintenanceRequests",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserID",
                table: "Notifications",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Software_LabID",
                table: "Software",
                column: "LabID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleID",
                table: "UserRoles",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserID",
                table: "UserRoles",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__A9D1053449081518",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabUsageLogs");

            migrationBuilder.DropTable(
                name: "MaintenanceRequests");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Software");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Computers");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Labs");
        }
    }
}
