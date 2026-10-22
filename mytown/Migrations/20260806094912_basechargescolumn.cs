using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class basechargescolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "destination",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "start_location",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "serv_subcat_image",
                table: "services_sub_categories");

            migrationBuilder.DropColumn(
                name: "serv_subcat_name",
                table: "services_sub_categories");

            migrationBuilder.DropColumn(
                name: "service_cost",
                table: "services");

            migrationBuilder.DropColumn(
                name: "service_description",
                table: "services");

            migrationBuilder.DropColumn(
                name: "service_image",
                table: "services");

            migrationBuilder.DropColumn(
                name: "service_subject",
                table: "services");

            migrationBuilder.RenameColumn(
                name: "busserv_id",
                table: "services_sub_categories",
                newName: "bus_serv_id");

            migrationBuilder.AddColumn<string>(
                name: "destination_city",
                table: "transporter_travel_plans",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "destination_country",
                table: "transporter_travel_plans",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "destination_state",
                table: "transporter_travel_plans",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "destination_town",
                table: "transporter_travel_plans",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "start_city",
                table: "transporter_travel_plans",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "start_country",
                table: "transporter_travel_plans",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "start_state",
                table: "transporter_travel_plans",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "start_town",
                table: "transporter_travel_plans",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "service_type_name",
                table: "services_sub_categories",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "service_name",
                table: "services",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "estimated_duration",
                table: "services",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "inspection_fee",
                table: "services",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "service_type_description",
                table: "services",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "service_type_image",
                table: "services",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "starting_price",
                table: "services",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sender_alternate_address",
                columns: table => new
                {
                    alt_address_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sender_reg_id = table.Column<int>(type: "int", nullable: false),
                    alt_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_phone_number = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_address = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_town = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_city = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_state = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_country = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_postal_code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    delivery_notes = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sender_alternate_address", x => x.alt_address_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "service_profiles",
                columns: table => new
                {
                    service_profile_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    bus_reg_id = table.Column<int>(type: "int", nullable: false),
                    bus_serv_id = table.Column<int>(type: "int", nullable: false),
                    business_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    business_location = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    service_description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    years_of_experience = table.Column<int>(type: "int", nullable: true),
                    govt_id_document = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    professional_license = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    service_available_locations = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    working_days = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    working_start_time = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    working_end_time = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    service_logo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    service_banner = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_profiles", x => x.service_profile_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sender_alternate_address");

            migrationBuilder.DropTable(
                name: "service_profiles");

            migrationBuilder.DropColumn(
                name: "destination_city",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "destination_country",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "destination_state",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "destination_town",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "start_city",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "start_country",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "start_state",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "start_town",
                table: "transporter_travel_plans");

            migrationBuilder.DropColumn(
                name: "service_type_name",
                table: "services_sub_categories");

            migrationBuilder.DropColumn(
                name: "estimated_duration",
                table: "services");

            migrationBuilder.DropColumn(
                name: "inspection_fee",
                table: "services");

            migrationBuilder.DropColumn(
                name: "service_type_description",
                table: "services");

            migrationBuilder.DropColumn(
                name: "service_type_image",
                table: "services");

            migrationBuilder.DropColumn(
                name: "starting_price",
                table: "services");

            migrationBuilder.RenameColumn(
                name: "bus_serv_id",
                table: "services_sub_categories",
                newName: "busserv_id");

            migrationBuilder.AddColumn<string>(
                name: "destination",
                table: "transporter_travel_plans",
                type: "varchar(300)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "start_location",
                table: "transporter_travel_plans",
                type: "varchar(300)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "serv_subcat_image",
                table: "services_sub_categories",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "serv_subcat_name",
                table: "services_sub_categories",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "service_name",
                table: "services",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "service_cost",
                table: "services",
                type: "decimal(10,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "service_description",
                table: "services",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "service_image",
                table: "services",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "service_subject",
                table: "services",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
