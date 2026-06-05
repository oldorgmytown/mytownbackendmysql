using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class datetoregsutration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingDetails_OrderDetails_OrderDetailId",
                table: "ShippingDetails");

            migrationBuilder.DropIndex(
                name: "IX_ShippingDetails_OrderDetailId",
                table: "ShippingDetails");

            migrationBuilder.AddColumn<DateTime>(
                name: "ShopperRegDate",
                table: "ShopperRegisters",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "BusinessRegDate",
                table: "BusinessRegisters",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShopperRegDate",
                table: "ShopperRegisters");

            migrationBuilder.DropColumn(
                name: "BusinessRegDate",
                table: "BusinessRegisters");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDetails_OrderDetailId",
                table: "ShippingDetails",
                column: "OrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingDetails_OrderDetails_OrderDetailId",
                table: "ShippingDetails",
                column: "OrderDetailId",
                principalTable: "OrderDetails",
                principalColumn: "OrderDetailId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
