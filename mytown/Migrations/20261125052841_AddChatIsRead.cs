using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class AddChatIsRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "order_transport_leg_id",
                table: "transporter_payout",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "leg_number",
                table: "hub_store_order_verification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "is_read",
                table: "chat_messages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order_transport_leg_id",
                table: "transporter_payout");

            migrationBuilder.DropColumn(
                name: "leg_number",
                table: "hub_store_order_verification");

            migrationBuilder.DropColumn(
                name: "is_read",
                table: "chat_messages");
        }
    }
}
