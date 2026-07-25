using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightRecorder.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFlightCallsign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "callsign",
                table: "FLIGHT",
                type: "VARCHAR(50)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "callsign",
                table: "FLIGHT");
        }
    }
}
