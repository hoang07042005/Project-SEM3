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
                name: "Equipments",
                columns: table => new
                {
                    EquipmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameEquipment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Equipmen__3447459958A3481B", x => x.EquipmentID);
                });

            migrationBuilder.CreateTable(
                name: "RequestImages",
                columns: table => new
                {
                    ImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RequestI__7516F4EC4DE8D026", x => x.ImageID);
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
                    table.PrimaryKey("PK__Roles__8AFACE3A4754D5DB", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "StatusLabs",
                columns: table => new
                {
                    StatusLabID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StatusLa__072A22DFB420707C", x => x.StatusLabID);
                });

            migrationBuilder.CreateTable(
                name: "StatusLog",
                columns: table => new
                {
                    StatusLogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StatusLog__5E1E5E8CE4764C2F", x => x.StatusLogID);
                });

            migrationBuilder.CreateTable(
                name: "StatusRequests",
                columns: table => new
                {
                    StatusRequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StatusRe__B62FB50E95575741", x => x.StatusRequestID);
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
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ResetToken = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TokenExpiry = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CCAC10AD3EDE", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Labs",
                columns: table => new
                {
                    LabID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    StatusLabID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Labs__EDBD773A77FA477F", x => x.LabID);
                    table.ForeignKey(
                        name: "FK__Labs__StatusLabI__45F365D3",
                        column: x => x.StatusLabID,
                        principalTable: "StatusLabs",
                        principalColumn: "StatusLabID",
                        onDelete: ReferentialAction.Cascade);
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
                    table.PrimaryKey("PK__UserRole__3D978A55B7812925", x => x.UserRoleID);
                    table.ForeignKey(
                        name: "FK__UserRoles__RoleI__412EB0B6",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__UserRoles__UserI__403A8C7D",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquiLabs",
                columns: table => new
                {
                    EquiLabID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentID = table.Column<int>(type: "int", nullable: false),
                    LabID = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EquiLabs__80AC09910675DFE1", x => x.EquiLabID);
                    table.ForeignKey(
                        name: "FK_EquiLabs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__EquiLabs__Equipm__4BAC3F29",
                        column: x => x.EquipmentID,
                        principalTable: "Equipments",
                        principalColumn: "EquipmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__EquiLabs__LabID__4CA06362",
                        column: x => x.LabID,
                        principalTable: "Labs",
                        principalColumn: "LabID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabUsageLogs",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusLogID = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LabUsage__5E5499A8EB6A0186", x => x.LogID);
                    table.ForeignKey(
                        name: "FK__LabUsageL__LabID__5812160E",
                        column: x => x.LabID,
                        principalTable: "Labs",
                        principalColumn: "LabID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__LabUsageL__StatusLogID__59FA5E80",
                        column: x => x.StatusLogID,
                        principalTable: "StatusLog",
                        principalColumn: "StatusLogID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__LabUsageL__UserI__59063A47",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    RequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabID = table.Column<int>(type: "int", nullable: false),
                    EquipmentID = table.Column<int>(type: "int", nullable: true),
                    ImageID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Requests__33A8519A00162311", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK__Requests__Equipm__628FA481",
                        column: x => x.EquipmentID,
                        principalTable: "Equipments",
                        principalColumn: "EquipmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Requests__ImageI__6383C8BA",
                        column: x => x.ImageID,
                        principalTable: "RequestImages",
                        principalColumn: "ImageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Requests__LabID__619B8048",
                        column: x => x.LabID,
                        principalTable: "Labs",
                        principalColumn: "LabID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Softwares",
                columns: table => new
                {
                    SoftwareID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    License = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Free"),
                    LabID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Software__25EDB8DCBF9069AC", x => x.SoftwareID);
                    table.ForeignKey(
                        name: "FK__Softwares__LabID__5441852A",
                        column: x => x.LabID,
                        principalTable: "Labs",
                        principalColumn: "LabID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoryRequests",
                columns: table => new
                {
                    HistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    StatusRequestID = table.Column<int>(type: "int", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HistoryR__4D7B4ADD8A14BF35", x => x.HistoryID);
                    table.ForeignKey(
                        name: "FK__HistoryRe__Reque__68487DD7",
                        column: x => x.RequestID,
                        principalTable: "Requests",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__HistoryRe__Statu__693CA210",
                        column: x => x.StatusRequestID,
                        principalTable: "StatusRequests",
                        principalColumn: "StatusRequestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__HistoryRe__UserI__6754599E",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Unread"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    RequestID = table.Column<int>(type: "int", nullable: true),
                    LabUsageLogID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__20CF2E3234B214C8", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notifications_LabUsageLogs_LabUsageLogID",
                        column: x => x.LabUsageLogID,
                        principalTable: "LabUsageLogs",
                        principalColumn: "LogID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Notificat__UserI__6EF57B66",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Notification__RequestId__F9F9A3A7",
                        column: x => x.RequestID,
                        principalTable: "Requests",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    FeedBackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Satisfaction = table.Column<int>(type: "int", nullable: false),
                    Quality = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Feedback__3447459958A3481B", x => x.FeedBackID);
                    table.ForeignKey(
                        name: "FK_Feedbacks_HistoryRequests_RequestID",
                        column: x => x.RequestID,
                        principalTable: "HistoryRequests",
                        principalColumn: "HistoryID");
                    table.ForeignKey(
                        name: "FK_Feedbacks_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquiLabs_EquipmentID",
                table: "EquiLabs",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EquiLabs_LabID",
                table: "EquiLabs",
                column: "LabID");

            migrationBuilder.CreateIndex(
                name: "IX_EquiLabs_UserId",
                table: "EquiLabs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RequestID",
                table: "Feedbacks",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserID",
                table: "Feedbacks",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRequests_RequestID",
                table: "HistoryRequests",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRequests_StatusRequestID",
                table: "HistoryRequests",
                column: "StatusRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryRequests_UserID",
                table: "HistoryRequests",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Labs_StatusLabID",
                table: "Labs",
                column: "StatusLabID");

            migrationBuilder.CreateIndex(
                name: "IX_LabUsageLogs_LabID",
                table: "LabUsageLogs",
                column: "LabID");

            migrationBuilder.CreateIndex(
                name: "IX_LabUsageLogs_StatusLogID",
                table: "LabUsageLogs",
                column: "StatusLogID");

            migrationBuilder.CreateIndex(
                name: "IX_LabUsageLogs_UserID",
                table: "LabUsageLogs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_LabUsageLogID",
                table: "Notifications",
                column: "LabUsageLogID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RequestID",
                table: "Notifications",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserID",
                table: "Notifications",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_EquipmentID",
                table: "Requests",
                column: "EquipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ImageID",
                table: "Requests",
                column: "ImageID");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_LabID",
                table: "Requests",
                column: "LabID");

            migrationBuilder.CreateIndex(
                name: "IX_Softwares_LabID",
                table: "Softwares",
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
                name: "UQ__Users__A9D105344E93A6A9",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquiLabs");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Softwares");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "HistoryRequests");

            migrationBuilder.DropTable(
                name: "LabUsageLogs");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "StatusRequests");

            migrationBuilder.DropTable(
                name: "StatusLog");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "RequestImages");

            migrationBuilder.DropTable(
                name: "Labs");

            migrationBuilder.DropTable(
                name: "StatusLabs");
        }
    }
}
