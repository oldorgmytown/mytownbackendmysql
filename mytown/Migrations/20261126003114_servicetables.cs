using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class servicetables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "order_transport_leg_id",
                table: "transporter_payout",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "leg_number",
                table: "hub_store_order_verification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "service_blocked_slots",
                columns: table => new
                {
                    blocked_slot_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    service_profile_id = table.Column<int>(type: "int", nullable: false),
                    blocked_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    end_time = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    reason = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_blocked_slots", x => x.blocked_slot_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "service_enquiries",
                columns: table => new
                {
                    enquiry_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shopper_reg_id = table.Column<int>(type: "int", nullable: false),
                    service_profile_id = table.Column<int>(type: "int", nullable: false),
                    bus_reg_id = table.Column<int>(type: "int", nullable: false),
                    bus_serv_id = table.Column<int>(type: "int", nullable: false),
                    service_id = table.Column<int>(type: "int", nullable: false),
                    service_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    number_of_units = table.Column<int>(type: "int", nullable: false),
                    preferred_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    preferred_time_slot = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    service_location = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    shopper_address_id = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    provider_response = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    confirmed_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    confirmed_time_slot = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    responded_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_enquiries", x => x.enquiry_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "service_blocked_slots");

            migrationBuilder.DropTable(
                name: "service_enquiries");

            migrationBuilder.DropColumn(
                name: "order_transport_leg_id",
                table: "transporter_payout");

            migrationBuilder.DropColumn(
                name: "leg_number",
                table: "hub_store_order_verification");
        }
    }
}
