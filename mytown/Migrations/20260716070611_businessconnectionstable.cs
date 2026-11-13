using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class businessconnectionstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "business_connections",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    bus_reg_id = table.Column<int>(type: "int", nullable: false),
                    shopper_reg_id = table.Column<int>(type: "int", nullable: false),
                    connected_on = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_connections", x => x.id);
                    table.ForeignKey(
                        name: "FK_business_connections_business_registers_bus_reg_id",
                        column: x => x.bus_reg_id,
                        principalTable: "business_registers",
                        principalColumn: "bus_reg_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_business_connections_shopper_registers_shopper_reg_id",
                        column: x => x.shopper_reg_id,
                        principalTable: "shopper_registers",
                        principalColumn: "shopper_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_business_connections_bus_reg_id",
                table: "business_connections",
                column: "bus_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_business_connections_shopper_reg_id",
                table: "business_connections",
                column: "shopper_reg_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "business_connections");
        }
    }
}
