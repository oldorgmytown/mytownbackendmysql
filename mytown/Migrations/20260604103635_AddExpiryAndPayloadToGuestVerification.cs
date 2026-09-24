using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class AddExpiryAndPayloadToGuestVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_pending_guest_verifications",
                table: "pending_guest_verifications");

            migrationBuilder.RenameTable(
                name: "pending_guest_verifications",
                newName: "pending_guest_verification");

            migrationBuilder.AddColumn<DateTime>(
                name: "expiry_date",
                table: "pending_guest_verification",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "json_payload",
                table: "pending_guest_verification",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pending_guest_verification",
                table: "pending_guest_verification",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_pending_guest_verification",
                table: "pending_guest_verification");

            migrationBuilder.DropColumn(
                name: "expiry_date",
                table: "pending_guest_verification");

            migrationBuilder.DropColumn(
                name: "json_payload",
                table: "pending_guest_verification");

            migrationBuilder.RenameTable(
                name: "pending_guest_verification",
                newName: "pending_guest_verifications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pending_guest_verifications",
                table: "pending_guest_verifications",
                column: "id");
        }
    }
}
