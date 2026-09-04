using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class basechargescolumnincourier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "base_charges",
                table: "courier_branch_service",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "base_charges",
                table: "courier_branch_service");
        }
    }
}
