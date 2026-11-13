using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class addtotransportertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "destination_pin",
                table: "transporter_travel_plans",
                type: "varchar(300)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "start_location_pin",
                table: "transporter_travel_plans",
                type: "varchar(300)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "chat_messages",
                columns: table => new
                {
                    chat_message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sender_user_id = table.Column<int>(type: "int", nullable: false),
                    sender_type = table.Column<int>(type: "int", nullable: false),
                    receiver_user_id = table.Column<int>(type: "int", nullable: false),
                    receiver_type = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sent_time = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_messages", x => x.chat_message_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_sender_user_id_sender_type_receiver_user_id_re~",
                table: "chat_messages",
                columns: new[] { "sender_user_id", "sender_type", "receiver_user_id", "receiver_type" });

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_sent_time",
                table: "chat_messages",
                column: "sent_time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_messages");

            migrationBuilder.DropColumn(
                name: "destination_pin",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "start_location_pin",
                table: "transporter_travel_plans");
        }
    }
}
