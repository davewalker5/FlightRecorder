using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace FlightRecorder.Data.Migrations
{
    /// <inheritdoc />
    public partial class DefaultLocation : Migration
    {
        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USER_ATTRIBUTE",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_ATTRIBUTE", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "USER_ATTRIBUTE_VALUE",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    user_attribute_id = table.Column<int>(type: "INTEGER", nullable: false),
                    value = table.Column<string>(type: "VARCHAR(1000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_ATTRIBUTE_VALUE", x => x.id);
                    table.ForeignKey(
                        name: "FK_USER_ATTRIBUTE_VALUE_USER_ATTRIBUTE_user_attribute_id",
                        column: x => x.user_attribute_id,
                        principalTable: "USER_ATTRIBUTE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USER_ATTRIBUTE_VALUE_USER_user_id",
                        column: x => x.user_id,
                        principalTable: "USER",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USER_ATTRIBUTE_VALUE_user_attribute_id",
                table: "USER_ATTRIBUTE_VALUE",
                column: "user_attribute_id");

            migrationBuilder.CreateIndex(
                name: "IX_USER_ATTRIBUTE_VALUE_user_id",
                table: "USER_ATTRIBUTE_VALUE",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "USER_ATTRIBUTE_VALUE");

            migrationBuilder.DropTable(
                name: "USER_ATTRIBUTE");
        }
    }
}
