using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class longattrubutecolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "attribute_id",
                table: "product_attributes",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "attribute_id",
                table: "product_attribute_values",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "attribute_value_id",
                table: "product_attribute_values",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_attributes_attribute_id",
                table: "product_variant_attributes",
                column: "attribute_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_attributes_attribute_value_id",
                table: "product_variant_attributes",
                column: "attribute_value_id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_variant_attributes_product_attribute_values_attribut~",
                table: "product_variant_attributes",
                column: "attribute_value_id",
                principalTable: "product_attribute_values",
                principalColumn: "attribute_value_id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_variant_attributes_product_attributes_attribute_id",
                table: "product_variant_attributes",
                column: "attribute_id",
                principalTable: "product_attributes",
                principalColumn: "attribute_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_attributes_product_attribute_values_attribut~",
                table: "product_variant_attributes");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_attributes_product_attributes_attribute_id",
                table: "product_variant_attributes");

            migrationBuilder.DropIndex(
                name: "IX_product_variant_attributes_attribute_id",
                table: "product_variant_attributes");

            migrationBuilder.DropIndex(
                name: "IX_product_variant_attributes_attribute_value_id",
                table: "product_variant_attributes");

            migrationBuilder.AlterColumn<int>(
                name: "attribute_id",
                table: "product_attributes",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "attribute_id",
                table: "product_attribute_values",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "attribute_value_id",
                table: "product_attribute_values",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
