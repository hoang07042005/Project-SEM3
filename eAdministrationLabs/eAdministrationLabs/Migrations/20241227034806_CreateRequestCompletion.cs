using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAdministrationLabs.Migrations
{
    /// <inheritdoc />
    public partial class CreateRequestCompletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestCompletions",
                columns: table => new
                {
                    CompletionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HistoryRequestID = table.Column<int>(type: "int", nullable: false),
                    CompletionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImageBase64 = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RequestCompletion__33A8519A00162311", x => x.CompletionID);
                    table.ForeignKey(
                        name: "FK_RequestCompletions_HistoryRequests_HistoryRequestID",
                        column: x => x.HistoryRequestID,
                        principalTable: "HistoryRequests",
                        principalColumn: "HistoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestCompletions_HistoryRequestID",
                table: "RequestCompletions",
                column: "HistoryRequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestCompletions");
        }
    }
}
