using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace FlightRecorder.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class OptionalAircraftDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIRCRAFT_MODEL_model_id",
                table: "AIRCRAFT");

            migrationBuilder.AlterColumn<string>(
                name: "serial_number",
                table: "AIRCRAFT",
                type: "VARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)");

            migrationBuilder.AlterColumn<long>(
                name: "model_id",
                table: "AIRCRAFT",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<long>(
                name: "manufactured",
                table: "AIRCRAFT",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.CreateTable(
                name: "AirlineStatistics",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Sightings = table.Column<int>(type: "INTEGER", nullable: true),
                    Flights = table.Column<int>(type: "INTEGER", nullable: true),
                    Locations = table.Column<int>(type: "INTEGER", nullable: true),
                    Aircraft = table.Column<int>(type: "INTEGER", nullable: true),
                    Models = table.Column<int>(type: "INTEGER", nullable: true),
                    Manufacturers = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "LocationStatistics",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Sightings = table.Column<int>(type: "INTEGER", nullable: true),
                    Flights = table.Column<int>(type: "INTEGER", nullable: true),
                    Aircraft = table.Column<int>(type: "INTEGER", nullable: true),
                    Models = table.Column<int>(type: "INTEGER", nullable: true),
                    Manufacturers = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ManufacturerStatistics",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Sightings = table.Column<int>(type: "INTEGER", nullable: true),
                    Flights = table.Column<int>(type: "INTEGER", nullable: true),
                    Locations = table.Column<int>(type: "INTEGER", nullable: true),
                    Aircraft = table.Column<int>(type: "INTEGER", nullable: true),
                    Models = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ModelStatistics",
                columns: table => new
                {
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: true),
                    Model = table.Column<string>(type: "TEXT", nullable: true),
                    Sightings = table.Column<int>(type: "INTEGER", nullable: true),
                    Flights = table.Column<int>(type: "INTEGER", nullable: true),
                    Locations = table.Column<int>(type: "INTEGER", nullable: true),
                    Aircraft = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AIRCRAFT_MODEL_model_id",
                table: "AIRCRAFT",
                column: "model_id",
                principalTable: "MODEL",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIRCRAFT_MODEL_model_id",
                table: "AIRCRAFT");

            migrationBuilder.DropTable(
                name: "AirlineStatistics");

            migrationBuilder.DropTable(
                name: "LocationStatistics");

            migrationBuilder.DropTable(
                name: "ManufacturerStatistics");

            migrationBuilder.DropTable(
                name: "ModelStatistics");

            migrationBuilder.AlterColumn<string>(
                name: "serial_number",
                table: "AIRCRAFT",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "model_id",
                table: "AIRCRAFT",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "manufactured",
                table: "AIRCRAFT",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AIRCRAFT_MODEL_model_id",
                table: "AIRCRAFT",
                column: "model_id",
                principalTable: "MODEL",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
