using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightRecorder.Data.Migrations
{
    /// <inheritdoc />
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    public partial class countries : Migration
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "COUNTRY",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COUNTRY", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AIRPORT",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    country_id = table.Column<long>(type: "INTEGER", nullable: false),
                    code = table.Column<string>(type: "VARCHAR(5)", nullable: false),
                    name = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIRPORT", x => x.id);
                    table.ForeignKey(
                        name: "FK_AIRPORT_COUNTRY_country_id",
                        column: x => x.country_id,
                        principalTable: "COUNTRY",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIRPORT_country_id",
                table: "AIRPORT",
                column: "country_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIRPORT");

            migrationBuilder.DropTable(
                name: "COUNTRY");
        }
    }
}
