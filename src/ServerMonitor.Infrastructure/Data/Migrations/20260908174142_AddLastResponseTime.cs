using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerMonitor.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLastResponseTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastResponseTimeMs",
                table: "Targets",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastResponseTimeMs",
                table: "Targets");
        }
    }
}
