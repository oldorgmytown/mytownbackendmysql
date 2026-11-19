using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class legstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "order_transport_legs",
                columns: table => new
                {
                    order_transport_leg_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shipping_detail_id = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    store_order_id = table.Column<int>(type: "int", nullable: false),
                    leg_number = table.Column<int>(type: "int", nullable: false),
                    is_final_leg = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    transporter_plan_id = table.Column<int>(type: "int", nullable: false),
                    transporter_reg_id = table.Column<int>(type: "int", nullable: false),
                    from_pin = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    to_pin = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    planned_start = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    planned_arrival = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    leg_status = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    hub_status = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    delivered_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    leg_cost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    is_paid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    paid_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_transport_legs", x => x.order_transport_leg_id);
                    table.ForeignKey(
                        name: "FK_order_transport_legs_shipping_details_shipping_detail_id",
                        column: x => x.shipping_detail_id,
                        principalTable: "shipping_details",
                        principalColumn: "shipping_detail_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shopper_connections",
                columns: table => new
                {
                    connection_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    requester_shopper_id = table.Column<int>(type: "int", nullable: false),
                    receiver_shopper_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    requested_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    responded_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopper_connections", x => x.connection_id);
                    table.ForeignKey(
                        name: "FK_shopper_connections_shopper_registers_receiver_shopper_id",
                        column: x => x.receiver_shopper_id,
                        principalTable: "shopper_registers",
                        principalColumn: "shopper_reg_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shopper_connections_shopper_registers_requester_shopper_id",
                        column: x => x.requester_shopper_id,
                        principalTable: "shopper_registers",
                        principalColumn: "shopper_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shopper_post_likes",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: false),
                    liked_by_shopper_id = table.Column<int>(type: "int", nullable: false),
                    liked_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopper_post_likes", x => new { x.post_id, x.liked_by_shopper_id });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shopper_posts",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shopper_reg_id = table.Column<int>(type: "int", nullable: false),
                    caption = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    bus_reg_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopper_posts", x => x.post_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shopper_post_images",
                columns: table => new
                {
                    image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    image_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<byte>(type: "tinyint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopper_post_images", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_shopper_post_images_shopper_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "shopper_posts",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_order_transport_legs_shipping_detail_id",
                table: "order_transport_legs",
                column: "shipping_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_shopper_connections_receiver_shopper_id",
                table: "shopper_connections",
                column: "receiver_shopper_id");

            migrationBuilder.CreateIndex(
                name: "IX_shopper_connections_requester_shopper_id",
                table: "shopper_connections",
                column: "requester_shopper_id");

            migrationBuilder.CreateIndex(
                name: "IX_shopper_post_images_post_id",
                table: "shopper_post_images",
                column: "post_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_transport_legs");

            migrationBuilder.DropTable(
                name: "shopper_connections");

            migrationBuilder.DropTable(
                name: "shopper_post_images");

            migrationBuilder.DropTable(
                name: "shopper_post_likes");

            migrationBuilder.DropTable(
                name: "shopper_posts");
        }
    }
}
