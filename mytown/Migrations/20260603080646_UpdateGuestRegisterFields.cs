using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGuestRegisterFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "guest_registers",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "guest_registers",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "guest_registers",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "postal_code",
                table: "guest_registers",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "guest_registers",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "town",
                table: "guest_registers",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "guest_registers");

            migrationBuilder.DropColumn(
                name: "city",
                table: "guest_registers");

            migrationBuilder.DropColumn(
                name: "country",
                table: "guest_registers");

            migrationBuilder.DropColumn(
                name: "postal_code",
                table: "guest_registers");

            migrationBuilder.DropColumn(
                name: "state",
                table: "guest_registers");

            migrationBuilder.DropColumn(
                name: "town",
                table: "guest_registers");
        }
    }
}
