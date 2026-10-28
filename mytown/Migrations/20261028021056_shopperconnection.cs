using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class shopperconnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ad_payment_orders",
                columns: table => new
                {
                    ad_payment_order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    bus_reg_id = table.Column<int>(type: "int", nullable: false),
                    budget = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    fee = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    gst = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    razorpay_order_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    razorpay_payment_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    provider = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    stripe_payment_intent_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    paid_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ad_payment_orders", x => x.ad_payment_order_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ad_promotions",
                columns: table => new
                {
                    promotion_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    bus_reg_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content_json = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    audience_json = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    thumbnail = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    media_url = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    placement = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    duration_days = table.Column<int>(type: "int", nullable: false),
                    budget = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    fee = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    gst = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    payment_order_id = table.Column<int>(type: "int", nullable: true),
                    transaction_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    approved_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    review_note = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ad_promotions", x => x.promotion_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "hub_sender_order_verification",
                columns: table => new
                {
                    verification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sender_order_id = table.Column<int>(type: "int", nullable: false),
                    hubid = table.Column<int>(type: "int", nullable: false),
                    hub_status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    package_verified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    security_check = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    travel_plan_verified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    transporter_verified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    package_handed_over = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    remarks = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hub_sender_order_verification", x => x.verification_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "hub_store_order_verification",
                columns: table => new
                {
                    verification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    store_order_id = table.Column<int>(type: "int", nullable: false),
                    hubid = table.Column<int>(type: "int", nullable: false),
                    hub_status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    package_verified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    security_check = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    travel_plan_verified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    transporter_verified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    package_handed_over = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    remarks = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hub_store_order_verification", x => x.verification_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ad_payment_orders_razorpay_order_id",
                table: "ad_payment_orders",
                column: "razorpay_order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ad_payment_orders_stripe_payment_intent_id",
                table: "ad_payment_orders",
                column: "stripe_payment_intent_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ad_promotions_bus_reg_id",
                table: "ad_promotions",
                column: "bus_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_ad_promotions_payment_order_id",
                table: "ad_promotions",
                column: "payment_order_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ad_payment_orders");

            migrationBuilder.DropTable(
                name: "ad_promotions");

            migrationBuilder.DropTable(
                name: "hub_sender_order_verification");

            migrationBuilder.DropTable(
                name: "hub_store_order_verification");
        }
    }
}
