using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlightRecorder.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIRLINE",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIRLINE", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "LOCATION",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOCATION", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MANUFACTURER",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MANUFACTURER", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "FLIGHT",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    airline_id = table.Column<long>(nullable: false),
                    number = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    embarkation = table.Column<string>(type: "VARCHAR(3)", nullable: false),
                    destination = table.Column<string>(type: "VARCHAR(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FLIGHT", x => x.id);
                    table.ForeignKey(
                        name: "FK_FLIGHT_AIRLINE_airline_id",
                        column: x => x.airline_id,
                        principalTable: "AIRLINE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MODEL",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    manufacturer_id = table.Column<long>(nullable: false),
                    name = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MODEL", x => x.id);
                    table.ForeignKey(
                        name: "FK_MODEL_MANUFACTURER_manufacturer_id",
                        column: x => x.manufacturer_id,
                        principalTable: "MANUFACTURER",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AIRCRAFT",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    model_id = table.Column<long>(nullable: false),
                    registration = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    serial_number = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    manufactured = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIRCRAFT", x => x.id);
                    table.ForeignKey(
                        name: "FK_AIRCRAFT_MODEL_model_id",
                        column: x => x.model_id,
                        principalTable: "MODEL",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SIGHTING",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    location_id = table.Column<long>(nullable: false),
                    flight_id = table.Column<long>(nullable: false),
                    aircraft_id = table.Column<long>(nullable: false),
                    altitude = table.Column<long>(nullable: false),
                    date = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIGHTING", x => x.id);
                    table.ForeignKey(
                        name: "FK_SIGHTING_AIRCRAFT_aircraft_id",
                        column: x => x.aircraft_id,
                        principalTable: "AIRCRAFT",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SIGHTING_FLIGHT_flight_id",
                        column: x => x.flight_id,
                        principalTable: "FLIGHT",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SIGHTING_LOCATION_location_id",
                        column: x => x.location_id,
                        principalTable: "LOCATION",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIRCRAFT_model_id",
                table: "AIRCRAFT",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "IX_FLIGHT_airline_id",
                table: "FLIGHT",
                column: "airline_id");

            migrationBuilder.CreateIndex(
                name: "IX_MODEL_manufacturer_id",
                table: "MODEL",
                column: "manufacturer_id");

            migrationBuilder.CreateIndex(
                name: "IX_SIGHTING_aircraft_id",
                table: "SIGHTING",
                column: "aircraft_id");

            migrationBuilder.CreateIndex(
                name: "IX_SIGHTING_flight_id",
                table: "SIGHTING",
                column: "flight_id");

            migrationBuilder.CreateIndex(
                name: "IX_SIGHTING_location_id",
                table: "SIGHTING",
                column: "location_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SIGHTING");

            migrationBuilder.DropTable(
                name: "AIRCRAFT");

            migrationBuilder.DropTable(
                name: "FLIGHT");

            migrationBuilder.DropTable(
                name: "LOCATION");

            migrationBuilder.DropTable(
                name: "MODEL");

            migrationBuilder.DropTable(
                name: "AIRLINE");

            migrationBuilder.DropTable(
                name: "MANUFACTURER");
        }
    }
}
