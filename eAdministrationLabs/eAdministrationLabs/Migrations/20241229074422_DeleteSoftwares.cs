using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAdministrationLabs.Migrations
{
    /// <inheritdoc />
    public partial class DeleteSoftwares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Softwares");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Softwares",
                columns: table => new
                {
                    SoftwareID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    License = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Free"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Softwares_LabID",
                table: "Softwares",
                column: "LabID");
        }
    }
}
