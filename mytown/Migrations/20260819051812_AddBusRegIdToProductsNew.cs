using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class AddBusRegIdToProductsNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "bus_reg_id",
                table: "products_new",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_products_new_bus_reg_id",
                table: "products_new",
                column: "bus_reg_id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_new_business_registers_bus_reg_id",
                table: "products_new",
                column: "bus_reg_id",
                principalTable: "business_registers",
                principalColumn: "bus_reg_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_new_business_registers_bus_reg_id",
                table: "products_new");

            migrationBuilder.DropIndex(
                name: "IX_products_new_bus_reg_id",
                table: "products_new");

            migrationBuilder.DropColumn(
                name: "bus_reg_id",
                table: "products_new");
        }
    }
}
