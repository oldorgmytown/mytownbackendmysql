using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShippingDetailIdFromPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shipping_details_courier_branch_branch_id",
                table: "shipping_details");

            migrationBuilder.AlterColumn<int>(
                name: "branch_id",
                table: "shipping_details",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "shipping_package_details",
                columns: table => new
                {
                    package_detail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    store_order_id = table.Column<int>(type: "int", nullable: false),
                    package_length = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    package_width = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    package_height = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    package_weight = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    dimension_unit = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    weight_unit = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipping_package_details", x => x.package_detail_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_shipping_details_transporter_reg_id",
                table: "shipping_details",
                column: "transporter_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_prod_subcat_id",
                table: "products",
                column: "prod_subcat_id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_product_sub_categories_prod_subcat_id",
                table: "products",
                column: "prod_subcat_id",
                principalTable: "product_sub_categories",
                principalColumn: "prod_subcat_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_shipping_details_courier_branch_branch_id",
                table: "shipping_details",
                column: "branch_id",
                principalTable: "courier_branch",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_shipping_details_transporter_registers_transporter_reg_id",
                table: "shipping_details",
                column: "transporter_reg_id",
                principalTable: "transporter_registers",
                principalColumn: "transporter_reg_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_product_sub_categories_prod_subcat_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_shipping_details_courier_branch_branch_id",
                table: "shipping_details");

            migrationBuilder.DropForeignKey(
                name: "FK_shipping_details_transporter_registers_transporter_reg_id",
                table: "shipping_details");

            migrationBuilder.DropTable(
                name: "shipping_package_details");

            migrationBuilder.DropIndex(
                name: "IX_shipping_details_transporter_reg_id",
                table: "shipping_details");

            migrationBuilder.DropIndex(
                name: "IX_products_prod_subcat_id",
                table: "products");

            migrationBuilder.AlterColumn<int>(
                name: "branch_id",
                table: "shipping_details",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_shipping_details_courier_branch_branch_id",
                table: "shipping_details",
                column: "branch_id",
                principalTable: "courier_branch",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
