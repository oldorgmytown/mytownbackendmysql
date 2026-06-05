using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class AddTransporterDashboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "transporter_bank_details",
                columns: table => new
                {
                    bank_detail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    transporter_reg_id = table.Column<int>(type: "int", nullable: false),
                    bank_name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account_number = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    branch_name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ifsc_code = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_verified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transporter_bank_details", x => x.bank_detail_id);
                    table.ForeignKey(
                        name: "FK_transporter_bank_details_transporter_registers_transporter_r~",
                        column: x => x.transporter_reg_id,
                        principalTable: "transporter_registers",
                        principalColumn: "transporter_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transporter_kyc",
                columns: table => new
                {
                    kyc_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    transporter_reg_id = table.Column<int>(type: "int", nullable: false),
                    document_type = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    document_number = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    document_file_name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kyc_status = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    submitted_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    reviewed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transporter_kyc", x => x.kyc_id);
                    table.ForeignKey(
                        name: "FK_transporter_kyc_transporter_registers_transporter_reg_id",
                        column: x => x.transporter_reg_id,
                        principalTable: "transporter_registers",
                        principalColumn: "transporter_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transporter_travel_plans",
                columns: table => new
                {
                    plan_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    transporter_reg_id = table.Column<int>(type: "int", nullable: false),
                    start_location = table.Column<string>(type: "varchar(300)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    destination = table.Column<string>(type: "varchar(300)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    preferred_route = table.Column<string>(type: "varchar(200)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    distance_km = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    arrival_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    vehicle_type = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    vehicle_registration = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    vehicle_name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    max_weight_kg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    package_size_l = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    package_size_w = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    package_size_h = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    number_of_packages = table.Column<int>(type: "int", nullable: false),
                    accepts_fragile = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    accepts_perishable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    preferred_contact = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    language_preference = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notify_new_orders = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    notify_payments = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    plan_status = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transporter_travel_plans", x => x.plan_id);
                    table.ForeignKey(
                        name: "FK_transporter_travel_plans_transporter_registers_transporter_r~",
                        column: x => x.transporter_reg_id,
                        principalTable: "transporter_registers",
                        principalColumn: "transporter_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transporter_delivery_requests",
                columns: table => new
                {
                    delivery_req_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    transporter_reg_id = table.Column<int>(type: "int", nullable: false),
                    shopper_reg_id = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: true),
                    pickup_location = table.Column<string>(type: "varchar(300)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dropoff_location = table.Column<string>(type: "varchar(300)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    package_weight_kg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    number_of_packages = table.Column<int>(type: "int", nullable: false),
                    delivery_fee = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    package_tags = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    delivery_status = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    accepted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    reached_pickup_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    picked_up_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    in_transit_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    delivered_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    delivery_proof_file = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transporter_delivery_requests", x => x.delivery_req_id);
                    table.ForeignKey(
                        name: "FK_transporter_delivery_requests_shopper_registers_shopper_reg_~",
                        column: x => x.shopper_reg_id,
                        principalTable: "shopper_registers",
                        principalColumn: "shopper_reg_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transporter_delivery_requests_transporter_registers_transpor~",
                        column: x => x.transporter_reg_id,
                        principalTable: "transporter_registers",
                        principalColumn: "transporter_reg_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transporter_delivery_requests_transporter_travel_plans_plan_~",
                        column: x => x.plan_id,
                        principalTable: "transporter_travel_plans",
                        principalColumn: "plan_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transporter_exception_reports",
                columns: table => new
                {
                    report_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    delivery_req_id = table.Column<int>(type: "int", nullable: false),
                    transporter_reg_id = table.Column<int>(type: "int", nullable: false),
                    exception_type = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reported_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_resolved = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transporter_exception_reports", x => x.report_id);
                    table.ForeignKey(
                        name: "FK_transporter_exception_reports_transporter_delivery_requests_~",
                        column: x => x.delivery_req_id,
                        principalTable: "transporter_delivery_requests",
                        principalColumn: "delivery_req_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transporter_exception_reports_transporter_registers_transpor~",
                        column: x => x.transporter_reg_id,
                        principalTable: "transporter_registers",
                        principalColumn: "transporter_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_transporter_bank_details_transporter_reg_id",
                table: "transporter_bank_details",
                column: "transporter_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_transporter_delivery_requests_plan_id",
                table: "transporter_delivery_requests",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_transporter_delivery_requests_shopper_reg_id",
                table: "transporter_delivery_requests",
                column: "shopper_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_transporter_delivery_requests_transporter_reg_id",
                table: "transporter_delivery_requests",
                column: "transporter_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_transporter_exception_reports_delivery_req_id",
                table: "transporter_exception_reports",
                column: "delivery_req_id");

            migrationBuilder.CreateIndex(
                name: "IX_transporter_exception_reports_transporter_reg_id",
                table: "transporter_exception_reports",
                column: "transporter_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_transporter_kyc_transporter_reg_id",
                table: "transporter_kyc",
                column: "transporter_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_transporter_travel_plans_transporter_reg_id",
                table: "transporter_travel_plans",
                column: "transporter_reg_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transporter_bank_details");

            migrationBuilder.DropTable(
                name: "transporter_exception_reports");

            migrationBuilder.DropTable(
                name: "transporter_kyc");

            migrationBuilder.DropTable(
                name: "transporter_delivery_requests");

            migrationBuilder.DropTable(
                name: "transporter_travel_plans");
        }
    }
}
