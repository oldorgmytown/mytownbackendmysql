using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryCodeAndAssignedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "accepted_at",
                table: "transporter_delivery_requests",
                newName: "assigned_at");

            migrationBuilder.AddColumn<string>(
                name: "delivery_code",
                table: "transporter_delivery_requests",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transporter_db_notifications",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    transporter_reg_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_read = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transporter_db_notifications", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_transporter_db_notifications_transporter_registers_transport~",
                        column: x => x.transporter_reg_id,
                        principalTable: "transporter_registers",
                        principalColumn: "transporter_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_transporter_db_notifications_transporter_reg_id",
                table: "transporter_db_notifications",
                column: "transporter_reg_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transporter_db_notifications");

            migrationBuilder.DropColumn(
                name: "delivery_code",
                table: "transporter_delivery_requests");

            migrationBuilder.RenameColumn(
                name: "assigned_at",
                table: "transporter_delivery_requests",
                newName: "accepted_at");
        }
    }
}
