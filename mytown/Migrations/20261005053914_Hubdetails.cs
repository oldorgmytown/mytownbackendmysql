using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class Hubdetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "destination_pin",
                table: "transporter_travel_plans",
                type: "varchar(300)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "start_location_pin",
                table: "transporter_travel_plans",
                type: "varchar(300)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "store_order_id",
                table: "transporter_payout",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "order_type",
                table: "transporter_payout",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "sender_order_id",
                table: "transporter_payout",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "transporter_charges",
                table: "sender_orders",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "hubdetails",
                columns: table => new
                {
                    hubid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    hubaddressid = table.Column<int>(type: "int", nullable: false),
                    hubname = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    hubemail = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    hubpassword = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    addressline = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    town = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    city = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    state = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    country = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    pin = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hubdetails", x => x.hubid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "product_reviews",
                columns: table => new
                {
                    product_review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shopper_reg_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    post_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rating = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    review = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_anonymous = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    verified_purchase = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_reviews", x => x.product_review_id);
                    table.ForeignKey(
                        name: "FK_product_reviews_products_new_product_id",
                        column: x => x.product_id,
                        principalTable: "products_new",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_reviews_shopper_registers_shopper_reg_id",
                        column: x => x.shopper_reg_id,
                        principalTable: "shopper_registers",
                        principalColumn: "shopper_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "product_review_photos",
                columns: table => new
                {
                    product_review_photo_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    product_review_id = table.Column<int>(type: "int", nullable: false),
                    photo_path = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_review_photos", x => x.product_review_photo_id);
                    table.ForeignKey(
                        name: "FK_product_review_photos_product_reviews_product_review_id",
                        column: x => x.product_review_id,
                        principalTable: "product_reviews",
                        principalColumn: "product_review_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_product_review_photos_product_review_id",
                table: "product_review_photos",
                column: "product_review_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_reviews_product_id",
                table: "product_reviews",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_reviews_shopper_reg_id",
                table: "product_reviews",
                column: "shopper_reg_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hubdetails");

            migrationBuilder.DropTable(
                name: "product_review_photos");

            migrationBuilder.DropTable(
                name: "product_reviews");

            migrationBuilder.DropColumn(
                name: "destination_pin",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "start_location_pin",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "order_type",
                table: "transporter_payout");

            migrationBuilder.DropColumn(
                name: "sender_order_id",
                table: "transporter_payout");

            migrationBuilder.AlterColumn<int>(
                name: "store_order_id",
                table: "transporter_payout",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "transporter_charges",
                table: "sender_orders",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);
        }
    }
}
