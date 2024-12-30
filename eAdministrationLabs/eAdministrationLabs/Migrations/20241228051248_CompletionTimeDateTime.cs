using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eAdministrationLabs.Migrations
{
    /// <inheritdoc />
    public partial class CompletionTimeDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletionTime",
                table: "RequestCompletions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "CompletionTime",
                table: "RequestCompletions",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
