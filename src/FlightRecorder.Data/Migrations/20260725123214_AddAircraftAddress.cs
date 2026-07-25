using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightRecorder.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAircraftAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "AIRCRAFT",
                type: "VARCHAR(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "AIRCRAFT");
        }
    }
}
