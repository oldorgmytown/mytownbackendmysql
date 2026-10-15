using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class skuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_attributes_product_variants_variant_id",
                table: "product_variant_attributes");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_images_product_variants_product_variant_vari~",
                table: "product_variant_images");

            migrationBuilder.DropIndex(
                name: "IX_product_variant_attributes_variant_id",
                table: "product_variant_attributes");

            migrationBuilder.RenameColumn(
                name: "variant_id",
                table: "product_variants",
                newName: "sku_id");

            migrationBuilder.RenameColumn(
                name: "product_variant_variant_id",
                table: "product_variant_images",
                newName: "product_variant_sku_id");

            migrationBuilder.RenameIndex(
                name: "IX_product_variant_images_product_variant_variant_id",
                table: "product_variant_images",
                newName: "IX_product_variant_images_product_variant_sku_id");

            migrationBuilder.RenameColumn(
                name: "variant_id",
                table: "product_variant_attributes",
                newName: "sku_id");

            migrationBuilder.AddColumn<long>(
                name: "sku_id",
                table: "product_variant_images",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "variant_sku_id",
                table: "product_variant_attributes",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_attributes_variant_sku_id",
                table: "product_variant_attributes",
                column: "variant_sku_id");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_attributes_product_variants_variant_sku_id",
                table: "product_variant_attributes");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_images_product_variants_product_variant_sku_~",
                table: "product_variant_images");

            migrationBuilder.DropIndex(
                name: "IX_product_variant_attributes_variant_sku_id",
                table: "product_variant_attributes");

            migrationBuilder.DropColumn(
                name: "sku_id",
                table: "product_variant_images");

            migrationBuilder.DropColumn(
                name: "variant_sku_id",
                table: "product_variant_attributes");

            migrationBuilder.RenameColumn(
                name: "sku_id",
                table: "product_variants",
                newName: "variant_id");

            migrationBuilder.RenameColumn(
                name: "product_variant_sku_id",
                table: "product_variant_images",
                newName: "product_variant_variant_id");

            migrationBuilder.RenameIndex(
                name: "IX_product_variant_images_product_variant_sku_id",
                table: "product_variant_images",
                newName: "IX_product_variant_images_product_variant_variant_id");

            migrationBuilder.RenameColumn(
                name: "sku_id",
                table: "product_variant_attributes",
                newName: "variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_attributes_variant_id",
                table: "product_variant_attributes",
                column: "variant_id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_variant_attributes_product_variants_variant_id",
                table: "product_variant_attributes",
                column: "variant_id",
                principalTable: "product_variants",
                principalColumn: "variant_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_variant_images_product_variants_product_variant_vari~",
                table: "product_variant_images",
                column: "product_variant_variant_id",
                principalTable: "product_variants",
                principalColumn: "variant_id");
        }
    }
}
