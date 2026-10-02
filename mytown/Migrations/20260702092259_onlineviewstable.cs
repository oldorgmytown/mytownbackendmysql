using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    public partial class onlineviewstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "business_profile_viewers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),

                    bus_reg_id = table.Column<int>(type: "int", nullable: false),

                    shopper_reg_id = table.Column<int>(type: "int", nullable: false),

                    last_seen = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_profile_viewers", x => x.id);

                    table.ForeignKey(
                        name: "FK_business_profile_viewers_business_registers_bus_reg_id",
                        column: x => x.bus_reg_id,
                        principalTable: "business_registers",
                        principalColumn: "bus_reg_id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_business_profile_viewers_shopper_registers_shopper_reg_id",
                        column: x => x.shopper_reg_id,
                        principalTable: "shopper_registers",
                        principalColumn: "shopper_reg_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_business_profile_viewers_bus_reg_id",
                table: "business_profile_viewers",
                column: "bus_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_business_profile_viewers_shopper_reg_id",
                table: "business_profile_viewers",
                column: "shopper_reg_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "business_profile_viewers");
        }
    }
}