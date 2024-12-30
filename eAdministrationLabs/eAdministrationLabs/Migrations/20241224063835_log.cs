using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAdministrationLabs.Migrations
{
    /// <inheritdoc />
    public partial class log : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "StatusLog",
                newName: "StatusLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "StatusLogs",
                newName: "StatusLog");
        }
    }
}
