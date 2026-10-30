using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreOrderIdToDeliveryRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "store_order_id",
                table: "transporter_delivery_requests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "notified",
                table: "shipping_package_details",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "shopper_db_notifications",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shopper_reg_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_read = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopper_db_notifications", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_shopper_db_notifications_shopper_registers_shopper_reg_id",
                        column: x => x.shopper_reg_id,
                        principalTable: "shopper_registers",
                        principalColumn: "shopper_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_transporter_delivery_requests_store_order_id",
                table: "transporter_delivery_requests",
                column: "store_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_shopper_db_notifications_shopper_reg_id",
                table: "shopper_db_notifications",
                column: "shopper_reg_id");

            migrationBuilder.AddForeignKey(
                name: "FK_transporter_delivery_requests_store_orders_store_order_id",
                table: "transporter_delivery_requests",
                column: "store_order_id",
                principalTable: "store_orders",
                principalColumn: "store_order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transporter_delivery_requests_store_orders_store_order_id",
                table: "transporter_delivery_requests");

            migrationBuilder.DropTable(
                name: "shopper_db_notifications");

            migrationBuilder.DropIndex(
                name: "IX_transporter_delivery_requests_store_order_id",
                table: "transporter_delivery_requests");

            migrationBuilder.DropColumn(
                name: "store_order_id",
                table: "transporter_delivery_requests");

            migrationBuilder.DropColumn(
                name: "notified",
                table: "shipping_package_details");
        }
    }
}
