using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAdministrationLabs.Migrations
{
    /// <inheritdoc />
    public partial class DeleteUserIdTableUserEquiLab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquiLabs_Users_UserId",
                table: "EquiLabs");

            migrationBuilder.DropIndex(
                name: "IX_EquiLabs_UserId",
                table: "EquiLabs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EquiLabs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "EquiLabs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquiLabs_UserId",
                table: "EquiLabs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquiLabs_Users_UserId",
                table: "EquiLabs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserID");
        }
    }
}
