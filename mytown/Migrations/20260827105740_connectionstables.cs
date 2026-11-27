using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class connectionstables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderdetails_products_product_id",
                table: "orderdetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orderdetails_sku_product_variants_sku_id",
                table: "orderdetails");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_attributes_product_variants_variant_sku_id",
                table: "product_variant_attributes");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_images_product_variants_product_variant_sku_~",
                table: "product_variant_images");

            migrationBuilder.DropForeignKey(
                name: "FK_shopper_product_recent_view_products_product_id",
                table: "shopper_product_recent_view");

            migrationBuilder.DropIndex(
                name: "IX_product_variant_images_product_variant_sku_id",
                table: "product_variant_images");

            migrationBuilder.DropIndex(
                name: "IX_product_variant_attributes_variant_sku_id",
                table: "product_variant_attributes");

            migrationBuilder.DropColumn(
                name: "product_variant_sku_id",
                table: "product_variant_images");

            migrationBuilder.DropColumn(
                name: "variant_id",
                table: "product_variant_images");

            migrationBuilder.DropColumn(
                name: "variant_sku_id",
                table: "product_variant_attributes");

            migrationBuilder.DropColumn(
                name: "bank_verification_message",
                table: "business_account_details");

            migrationBuilder.DropColumn(
                name: "bank_verification_reference",
                table: "business_account_details");

            migrationBuilder.DropColumn(
                name: "bank_verification_status",
                table: "business_account_details");

            migrationBuilder.DropColumn(
                name: "bank_verified_date",
                table: "business_account_details");

            migrationBuilder.DropColumn(
                name: "contact_created_date",
                table: "business_account_details");

            migrationBuilder.DropColumn(
                name: "fund_account_created_date",
                table: "business_account_details");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "business_account_details");

            migrationBuilder.DropColumn(
                name: "razorpay_contact_id",
                table: "business_account_details");

            migrationBuilder.DropColumn(
                name: "razorpay_contact_status",
                table: "business_account_details");

            migrationBuilder.DropColumn(
                name: "razorpay_fund_account_id",
                table: "business_account_details");

            migrationBuilder.DropColumn(
                name: "razorpay_fund_account_status",
                table: "business_account_details");

            migrationBuilder.DropColumn(
                name: "remarks",
                table: "business_account_details");

            migrationBuilder.AlterColumn<long>(
                name: "product_id",
                table: "shopper_product_recent_view",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "sku_id",
                table: "orderdetails",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "product_id",
                table: "orderdetails",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "shopper_experience_comments",
                columns: table => new
                {
                    shopper_experience_comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shopper_experience_id = table.Column<int>(type: "int", nullable: false),
                    shopper_reg_id = table.Column<int>(type: "int", nullable: false),
                    comment_text = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_anonymous = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopper_experience_comments", x => x.shopper_experience_comment_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shopper_experience_likes",
                columns: table => new
                {
                    shopper_experience_like_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shopper_experience_id = table.Column<int>(type: "int", nullable: false),
                    shopper_reg_id = table.Column<int>(type: "int", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopper_experience_likes", x => x.shopper_experience_like_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shopper_experience_photos",
                columns: table => new
                {
                    shopper_experience_photo_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shopper_experience_id = table.Column<int>(type: "int", nullable: false),
                    photo_url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopper_experience_photos", x => x.shopper_experience_photo_id);
                    table.ForeignKey(
                        name: "FK_shopper_experience_photos_shopper_experiences_shopper_experi~",
                        column: x => x.shopper_experience_id,
                        principalTable: "shopper_experiences",
                        principalColumn: "shopper_experience_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_images_sku_id",
                table: "product_variant_images",
                column: "sku_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_attributes_sku_id",
                table: "product_variant_attributes",
                column: "sku_id");

            migrationBuilder.CreateIndex(
                name: "IX_shopper_experience_photos_shopper_experience_id",
                table: "shopper_experience_photos",
                column: "shopper_experience_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orderdetails_product_variants_sku_id",
                table: "orderdetails",
                column: "sku_id",
                principalTable: "product_variants",
                principalColumn: "sku_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderdetails_products_new_product_id",
                table: "orderdetails",
                column: "product_id",
                principalTable: "products_new",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_variant_attributes_product_variants_sku_id",
                table: "product_variant_attributes",
                column: "sku_id",
                principalTable: "product_variants",
                principalColumn: "sku_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_variant_images_product_variants_sku_id",
                table: "product_variant_images",
                column: "sku_id",
                principalTable: "product_variants",
                principalColumn: "sku_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_shopper_product_recent_view_products_new_product_id",
                table: "shopper_product_recent_view",
                column: "product_id",
                principalTable: "products_new",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderdetails_product_variants_sku_id",
                table: "orderdetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orderdetails_products_new_product_id",
                table: "orderdetails");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_attributes_product_variants_sku_id",
                table: "product_variant_attributes");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_images_product_variants_sku_id",
                table: "product_variant_images");

            migrationBuilder.DropForeignKey(
                name: "FK_shopper_product_recent_view_products_new_product_id",
                table: "shopper_product_recent_view");

            migrationBuilder.DropTable(
                name: "shopper_experience_comments");

            migrationBuilder.DropTable(
                name: "shopper_experience_likes");

            migrationBuilder.DropTable(
                name: "shopper_experience_photos");

            migrationBuilder.DropIndex(
                name: "IX_product_variant_images_sku_id",
                table: "product_variant_images");

            migrationBuilder.DropIndex(
                name: "IX_product_variant_attributes_sku_id",
                table: "product_variant_attributes");

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "shopper_product_recent_view",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "product_variant_sku_id",
                table: "product_variant_images",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "variant_id",
                table: "product_variant_images",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "variant_sku_id",
                table: "product_variant_attributes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "sku_id",
                table: "orderdetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "product_id",
                table: "orderdetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "bank_verification_message",
                table: "business_account_details",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "bank_verification_reference",
                table: "business_account_details",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "bank_verification_status",
                table: "business_account_details",
                type: "int",
                maxLength: 20,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "bank_verified_date",
                table: "business_account_details",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "contact_created_date",
                table: "business_account_details",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fund_account_created_date",
                table: "business_account_details",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "business_account_details",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "razorpay_contact_id",
                table: "business_account_details",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "razorpay_contact_status",
                table: "business_account_details",
                type: "int",
                maxLength: 20,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "razorpay_fund_account_id",
                table: "business_account_details",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "razorpay_fund_account_status",
                table: "business_account_details",
                type: "int",
                maxLength: 20,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "remarks",
                table: "business_account_details",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_images_product_variant_sku_id",
                table: "product_variant_images",
                column: "product_variant_sku_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_attributes_variant_sku_id",
                table: "product_variant_attributes",
                column: "variant_sku_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orderdetails_products_product_id",
                table: "orderdetails",
                column: "product_id",
                principalTable: "products",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderdetails_sku_product_variants_sku_id",
                table: "orderdetails",
                column: "sku_id",
                principalTable: "sku_product_variants",
                principalColumn: "sku_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_variant_attributes_product_variants_variant_sku_id",
                table: "product_variant_attributes",
                column: "variant_sku_id",
                principalTable: "product_variants",
                principalColumn: "sku_id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_variant_images_product_variants_product_variant_sku_~",
                table: "product_variant_images",
                column: "product_variant_sku_id",
                principalTable: "product_variants",
                principalColumn: "sku_id");

            migrationBuilder.AddForeignKey(
                name: "FK_shopper_product_recent_view_products_product_id",
                table: "shopper_product_recent_view",
                column: "product_id",
                principalTable: "products",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
