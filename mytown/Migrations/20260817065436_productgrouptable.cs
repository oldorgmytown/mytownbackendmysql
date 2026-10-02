using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class productgrouptable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "prod_group_id",
                table: "product_type",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "base_charges",
                table: "courier_branch_service",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "prod_group_id",
                table: "product_type");

            migrationBuilder.DropColumn(
                name: "base_charges",
                table: "courier_branch_service");

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
        }
    }
}
