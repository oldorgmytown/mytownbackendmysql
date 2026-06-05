using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace mytown.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessVerifications_BusinessRegisters_BusinessBusRegId",
                table: "BusinessVerifications");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_BusinessRegisters_StoreId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_products_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ShopperRegisters_ShopperRegId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingDetails_Orders_OrderId",
                table: "ShippingDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopperVerification_ShopperRegisters_ShopperId",
                table: "ShopperVerification");

            migrationBuilder.DropTable(
                name: "BusinessCategories");

            migrationBuilder.DropTable(
                name: "BusinessServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subcategoryimages_Busregids",
                table: "subcategoryimages_Busregids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Registrations",
                table: "Registrations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDetails",
                table: "OrderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopperVerification",
                table: "ShopperVerification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopperRegisters",
                table: "ShopperRegisters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShippingDetails",
                table: "ShippingDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PendingVerifications",
                table: "PendingVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PendingCourierVerifications",
                table: "PendingCourierVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PendingBusinessVerifications",
                table: "PendingBusinessVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PasswordResetRequests",
                table: "PasswordResetRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourierService",
                table: "CourierService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessVerifications",
                table: "BusinessVerifications");

            migrationBuilder.DropIndex(
                name: "IX_BusinessVerifications_BusinessBusRegId",
                table: "BusinessVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessRegisters",
                table: "BusinessRegisters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessProfiles",
                table: "BusinessProfiles");

            migrationBuilder.DropColumn(
                name: "product_cost",
                table: "products");

            migrationBuilder.DropColumn(
                name: "product_height",
                table: "products");

            migrationBuilder.DropColumn(
                name: "product_length",
                table: "products");

            migrationBuilder.DropColumn(
                name: "product_quantity",
                table: "products");

            migrationBuilder.DropColumn(
                name: "product_weight",
                table: "products");

            migrationBuilder.DropColumn(
                name: "product_width",
                table: "products");

            migrationBuilder.DropColumn(
                name: "AadharNumber",
                table: "CourierService");

            migrationBuilder.DropColumn(
                name: "CourierAddress",
                table: "CourierService");

            migrationBuilder.DropColumn(
                name: "CourierCity",
                table: "CourierService");

            migrationBuilder.DropColumn(
                name: "CourierCountry",
                table: "CourierService");

            migrationBuilder.DropColumn(
                name: "CourierState",
                table: "CourierService");

            migrationBuilder.DropColumn(
                name: "CourierTown",
                table: "CourierService");

            migrationBuilder.DropColumn(
                name: "BusinessBusRegId",
                table: "BusinessVerifications");

            migrationBuilder.DropColumn(
                name: "BusinessUsername",
                table: "BusinessProfiles");

            migrationBuilder.DropColumn(
                name: "Businesscategory_name",
                table: "BusinessProfiles");

            migrationBuilder.DropColumn(
                name: "Businessservice_name",
                table: "BusinessProfiles");

            migrationBuilder.DropColumn(
                name: "bus_time",
                table: "BusinessProfiles");

            migrationBuilder.DropColumn(
                name: "image_positionx",
                table: "BusinessProfiles");

            migrationBuilder.DropColumn(
                name: "image_positiony",
                table: "BusinessProfiles");

            migrationBuilder.DropColumn(
                name: "zoom",
                table: "BusinessProfiles");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "subcategoryimages_Busregids",
                newName: "subcategoryimages_busregids");

            migrationBuilder.RenameTable(
                name: "Registrations",
                newName: "registrations");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "payments");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "orders");

            migrationBuilder.RenameTable(
                name: "OrderDetails",
                newName: "orderdetails");

            migrationBuilder.RenameTable(
                name: "ShopperVerification",
                newName: "shopper_verification");

            migrationBuilder.RenameTable(
                name: "ShopperRegisters",
                newName: "shopper_registers");

            migrationBuilder.RenameTable(
                name: "ShippingDetails",
                newName: "shipping_details");

            migrationBuilder.RenameTable(
                name: "PendingVerifications",
                newName: "pending_verifications");

            migrationBuilder.RenameTable(
                name: "PendingCourierVerifications",
                newName: "pending_courier_verifications");

            migrationBuilder.RenameTable(
                name: "PendingBusinessVerifications",
                newName: "pending_business_verifications");

            migrationBuilder.RenameTable(
                name: "PasswordResetRequests",
                newName: "password_reset_requests");

            migrationBuilder.RenameTable(
                name: "CourierService",
                newName: "courier_service");

            migrationBuilder.RenameTable(
                name: "BusinessVerifications",
                newName: "business_verifications");

            migrationBuilder.RenameTable(
                name: "BusinessRegisters",
                newName: "business_registers");

            migrationBuilder.RenameTable(
                name: "BusinessProfiles",
                newName: "business_profiles");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "users",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "users",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "users",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Prod_subcat_name",
                table: "subcategoryimages_busregids",
                newName: "prod_subcat_name");

            migrationBuilder.RenameColumn(
                name: "Prod_subcat_image",
                table: "subcategoryimages_busregids",
                newName: "prod_subcat_image");

            migrationBuilder.RenameColumn(
                name: "Prod_subcat_id",
                table: "subcategoryimages_busregids",
                newName: "prod_subcat_id");

            migrationBuilder.RenameColumn(
                name: "Image_Id",
                table: "subcategoryimages_busregids",
                newName: "image_id");

            migrationBuilder.RenameColumn(
                name: "BuscatId",
                table: "subcategoryimages_busregids",
                newName: "buscat_id");

            migrationBuilder.RenameColumn(
                name: "BusRegId",
                table: "subcategoryimages_busregids",
                newName: "bus_reg_id");

            migrationBuilder.RenameColumn(
                name: "BusservId",
                table: "services_sub_categories",
                newName: "busserv_id");

            migrationBuilder.RenameColumn(
                name: "BusservId",
                table: "services",
                newName: "bus_serv_id");

            migrationBuilder.RenameColumn(
                name: "BusRegId",
                table: "services",
                newName: "bus_reg_id");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "registrations",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Town",
                table: "registrations",
                newName: "town");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "registrations",
                newName: "state");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "registrations",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Otp",
                table: "registrations",
                newName: "otp");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "registrations",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Dob",
                table: "registrations",
                newName: "dob");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "registrations",
                newName: "country");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "registrations",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "PhoneNo",
                table: "registrations",
                newName: "phone_no");

            migrationBuilder.RenameColumn(
                name: "NewPassword",
                table: "registrations",
                newName: "new_password");

            migrationBuilder.RenameColumn(
                name: "CnfPassword",
                table: "registrations",
                newName: "cnf_password");

            migrationBuilder.RenameColumn(
                name: "RegId",
                table: "registrations",
                newName: "reg_id");

            migrationBuilder.RenameColumn(
                name: "BuscatId",
                table: "products",
                newName: "buscat_id");

            migrationBuilder.RenameColumn(
                name: "BusRegId",
                table: "products",
                newName: "bus_reg_id");

            migrationBuilder.RenameColumn(
                name: "BuscatId",
                table: "product_sub_categories",
                newName: "bus_cat_id");

            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                table: "payments",
                newName: "payment_status");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "payments",
                newName: "payment_method");

            migrationBuilder.RenameColumn(
                name: "PaymentDate",
                table: "payments",
                newName: "payment_date");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "payments",
                newName: "order_id");

            migrationBuilder.RenameColumn(
                name: "AmountPaid",
                table: "payments",
                newName: "amount_paid");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "payments",
                newName: "payment_id");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_OrderId",
                table: "payments",
                newName: "IX_payments_order_id");

            migrationBuilder.RenameColumn(
                name: "OrderStatus",
                table: "orders",
                newName: "orderstatus");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "orders",
                newName: "total_amount");

            migrationBuilder.RenameColumn(
                name: "ShopperRegId",
                table: "orders",
                newName: "shopper_reg_id");

            migrationBuilder.RenameColumn(
                name: "ShippingType",
                table: "orders",
                newName: "shipping_type");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "orders",
                newName: "order_date");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "orders",
                newName: "order_id");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_ShopperRegId",
                table: "orders",
                newName: "IX_orders_shopper_reg_id");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "orderdetails",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "orderdetails",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "orderdetails",
                newName: "store_id");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "orderdetails",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "orderdetails",
                newName: "order_id");

            migrationBuilder.RenameColumn(
                name: "OrderDetailId",
                table: "orderdetails",
                newName: "order_detail_id");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_StoreId",
                table: "orderdetails",
                newName: "IX_orderdetails_store_id");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ProductId",
                table: "orderdetails",
                newName: "IX_orderdetails_product_id");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_OrderId",
                table: "orderdetails",
                newName: "IX_orderdetails_order_id");

            migrationBuilder.RenameColumn(
                name: "ShopperRegId",
                table: "addtocart",
                newName: "shopper_reg_id");

            migrationBuilder.RenameColumn(
                name: "BuscatId",
                table: "addtocart",
                newName: "buscat_id");

            migrationBuilder.RenameColumn(
                name: "BusRegId",
                table: "addtocart",
                newName: "bus_reg_id");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "addtocart",
                newName: "cart_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "shopper_verification",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "VerificationToken",
                table: "shopper_verification",
                newName: "verification_token");

            migrationBuilder.RenameColumn(
                name: "ShopperId",
                table: "shopper_verification",
                newName: "shopper_id");

            migrationBuilder.RenameColumn(
                name: "IsUsed",
                table: "shopper_verification",
                newName: "is_used");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "shopper_verification",
                newName: "expiry_date");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "shopper_verification",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_ShopperVerification_ShopperId",
                table: "shopper_verification",
                newName: "IX_shopper_verification_shopper_id");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "shopper_registers",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Town",
                table: "shopper_registers",
                newName: "town");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "shopper_registers",
                newName: "state");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "shopper_registers",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "shopper_registers",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "shopper_registers",
                newName: "country");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "shopper_registers",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "shopper_registers",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "ShopperRegDate",
                table: "shopper_registers",
                newName: "shopper_reg_date");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "shopper_registers",
                newName: "postal_code");

            migrationBuilder.RenameColumn(
                name: "PhotoName",
                table: "shopper_registers",
                newName: "photo_name");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "shopper_registers",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "IsEmailVerified",
                table: "shopper_registers",
                newName: "is_email_verified");

            migrationBuilder.RenameColumn(
                name: "ShopperRegId",
                table: "shopper_registers",
                newName: "shopper_reg_id");

            migrationBuilder.RenameColumn(
                name: "Shipping_type",
                table: "shipping_details",
                newName: "shipping_type");

            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "shipping_details",
                newName: "cost");

            migrationBuilder.RenameColumn(
                name: "TrackingId",
                table: "shipping_details",
                newName: "tracking_id");

            migrationBuilder.RenameColumn(
                name: "ShippingStatus",
                table: "shipping_details",
                newName: "shipping_status");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "shipping_details",
                newName: "order_id");

            migrationBuilder.RenameColumn(
                name: "EstimatedDays",
                table: "shipping_details",
                newName: "estimated_days");

            migrationBuilder.RenameColumn(
                name: "ShippingDetailId",
                table: "shipping_details",
                newName: "shipping_detail_id");

            migrationBuilder.RenameColumn(
                name: "OrderDetailId",
                table: "shipping_details",
                newName: "store_order_id");

            migrationBuilder.RenameIndex(
                name: "IX_ShippingDetails_OrderId",
                table: "shipping_details",
                newName: "IX_shipping_details_order_id");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "pending_verifications",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "pending_verifications",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "pending_verifications",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "JsonPayload",
                table: "pending_verifications",
                newName: "json_payload");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "pending_verifications",
                newName: "expiry_date");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "pending_courier_verifications",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "pending_courier_verifications",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "pending_courier_verifications",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "JsonPayload",
                table: "pending_courier_verifications",
                newName: "json_payload");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "pending_courier_verifications",
                newName: "expiry_date");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "pending_business_verifications",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "pending_business_verifications",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "pending_business_verifications",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "JsonPayload",
                table: "pending_business_verifications",
                newName: "json_payload");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "pending_business_verifications",
                newName: "expiry_date");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "password_reset_requests",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Expiry",
                table: "password_reset_requests",
                newName: "expiry");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "password_reset_requests",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "password_reset_requests",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "courier_service",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "RegisteredDate",
                table: "courier_service",
                newName: "registered_date");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "courier_service",
                newName: "postal_code");

            migrationBuilder.RenameColumn(
                name: "IsEmailVerified",
                table: "courier_service",
                newName: "is_email_verified");

            migrationBuilder.RenameColumn(
                name: "CourierServiceName",
                table: "courier_service",
                newName: "courier_service_name");

            migrationBuilder.RenameColumn(
                name: "CourierPhone",
                table: "courier_service",
                newName: "courier_phone");

            migrationBuilder.RenameColumn(
                name: "CourierEmail",
                table: "courier_service",
                newName: "courier_email");

            migrationBuilder.RenameColumn(
                name: "CourierId",
                table: "courier_service",
                newName: "courier_id");

            migrationBuilder.RenameColumn(
                name: "LicenseNumber",
                table: "courier_service",
                newName: "profile_status");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "business_verifications",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "VerificationToken",
                table: "business_verifications",
                newName: "verification_token");

            migrationBuilder.RenameColumn(
                name: "IsUsed",
                table: "business_verifications",
                newName: "is_used");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "business_verifications",
                newName: "expiry_date");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "business_verifications",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "BusRegId",
                table: "business_verifications",
                newName: "bus_reg_id");

            migrationBuilder.RenameColumn(
                name: "Town",
                table: "business_registers",
                newName: "town");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "business_registers",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Gstin",
                table: "business_registers",
                newName: "gstin");

            migrationBuilder.RenameColumn(
                name: "postalCode",
                table: "business_registers",
                newName: "postal_code");

            migrationBuilder.RenameColumn(
                name: "businessState",
                table: "business_registers",
                newName: "business_state");

            migrationBuilder.RenameColumn(
                name: "businessCountry",
                table: "business_registers",
                newName: "business_country");

            migrationBuilder.RenameColumn(
                name: "businessCity",
                table: "business_registers",
                newName: "business_city");

            migrationBuilder.RenameColumn(
                name: "LicenseType",
                table: "business_registers",
                newName: "license_type");

            migrationBuilder.RenameColumn(
                name: "IsEmailVerified",
                table: "business_registers",
                newName: "is_email_verified");

            migrationBuilder.RenameColumn(
                name: "BusservId",
                table: "business_registers",
                newName: "bus_serv_id");

            migrationBuilder.RenameColumn(
                name: "Businessname",
                table: "business_registers",
                newName: "business_name");

            migrationBuilder.RenameColumn(
                name: "BusinessUsername",
                table: "business_registers",
                newName: "business_username");

            migrationBuilder.RenameColumn(
                name: "BusinessRegDate",
                table: "business_registers",
                newName: "business_reg_date");

            migrationBuilder.RenameColumn(
                name: "BuscatId",
                table: "business_registers",
                newName: "bus_cat_id");

            migrationBuilder.RenameColumn(
                name: "BusMobileNo",
                table: "business_registers",
                newName: "bus_mobile_no");

            migrationBuilder.RenameColumn(
                name: "BusEmail",
                table: "business_registers",
                newName: "bus_email");

            migrationBuilder.RenameColumn(
                name: "Address2",
                table: "business_registers",
                newName: "address_2");

            migrationBuilder.RenameColumn(
                name: "Address1",
                table: "business_registers",
                newName: "address_1");

            migrationBuilder.RenameColumn(
                name: "BusRegId",
                table: "business_registers",
                newName: "bus_reg_id");

            migrationBuilder.RenameColumn(
                name: "BusServId",
                table: "business_profiles",
                newName: "bus_serv_id");

            migrationBuilder.RenameColumn(
                name: "BusRegId",
                table: "business_profiles",
                newName: "bus_reg_id");

            migrationBuilder.RenameColumn(
                name: "BusCatId",
                table: "business_profiles",
                newName: "bus_cat_id");

            migrationBuilder.RenameColumn(
                name: "businessprofile_id",
                table: "business_profiles",
                newName: "business_profile_id");

            migrationBuilder.AlterColumn<decimal>(
                name: "service_cost",
                table: "services",
                type: "decimal(10,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "product_subject",
                table: "products",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "product_image",
                table: "products",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "design_id",
                table: "products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fabric_id",
                table: "products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "products",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "product_status",
                table: "products",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "product_type_id",
                table: "products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "supplier_name",
                table: "products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "prod_subcat_image",
                table: "product_sub_categories",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "stripe_payment_intent_id",
                table: "payments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "selected_alt_address_id",
                table: "orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sku_id",
                table: "orderdetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "store_order_id",
                table: "orderdetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sku_id",
                table: "addtocart",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "shopper_registers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "shipping_type",
                table: "shipping_details",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "cost",
                table: "shipping_details",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<string>(
                name: "tracking_id",
                table: "shipping_details",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "shipping_status",
                table: "shipping_details",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "branch_id",
                table: "shipping_details",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "delivered_date",
                table: "shipping_details",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "delivery_address",
                table: "shipping_details",
                type: "text",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "delivery_proof_file_name",
                table: "shipping_details",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "courier_service",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "postal_code",
                table: "courier_service",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "courier_service_name",
                table: "courier_service",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "courier_service",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "courier_service",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "courier_service",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "courier_website_name",
                table: "courier_service",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "is_city",
                table: "courier_service",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_state",
                table: "courier_service",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "courier_service",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "town",
                table: "courier_service",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "address_2",
                table: "business_registers",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "business_registers",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "profile_status",
                table: "business_profiles",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "business_location",
                table: "business_profiles",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "business_about",
                table: "business_profiles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "banner_path",
                table: "business_profiles",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "approved_date",
                table: "business_profiles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "business_name",
                table: "business_profiles",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "logo_path",
                table: "business_profiles",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subcategoryimages_busregids",
                table: "subcategoryimages_busregids",
                column: "image_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_registrations",
                table: "registrations",
                column: "reg_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payments",
                table: "payments",
                column: "payment_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_orders",
                table: "orders",
                column: "order_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_orderdetails",
                table: "orderdetails",
                column: "order_detail_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shopper_verification",
                table: "shopper_verification",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shopper_registers",
                table: "shopper_registers",
                column: "shopper_reg_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shipping_details",
                table: "shipping_details",
                column: "shipping_detail_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pending_verifications",
                table: "pending_verifications",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pending_courier_verifications",
                table: "pending_courier_verifications",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pending_business_verifications",
                table: "pending_business_verifications",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_password_reset_requests",
                table: "password_reset_requests",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_courier_service",
                table: "courier_service",
                column: "courier_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_business_verifications",
                table: "business_verifications",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_business_registers",
                table: "business_registers",
                column: "bus_reg_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_business_profiles",
                table: "business_profiles",
                column: "business_profile_id");

            migrationBuilder.CreateTable(
                name: "admin_comments",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    bus_reg_id = table.Column<int>(type: "int", nullable: false),
                    comments = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_comments", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK_admin_comments_business_registers_bus_reg_id",
                        column: x => x.bus_reg_id,
                        principalTable: "business_registers",
                        principalColumn: "bus_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "business_categories",
                columns: table => new
                {
                    bus_cat_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    business_category_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_categories", x => x.bus_cat_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "business_db_notifications",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    bus_reg_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_read = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_db_notifications", x => x.notification_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "business_services",
                columns: table => new
                {
                    bus_serv_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    business_service_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_services", x => x.bus_serv_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "courier_branch",
                columns: table => new
                {
                    branch_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    courier_id = table.Column<int>(type: "int", nullable: false),
                    courier_service_name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    country = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    state = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    city = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    town = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    branch_phone_number = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    branch_email_id = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    branch_contact_person = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password_hash = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courier_branch", x => x.branch_id);
                    table.ForeignKey(
                        name: "FK_courier_branch_courier_service_courier_id",
                        column: x => x.courier_id,
                        principalTable: "courier_service",
                        principalColumn: "courier_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "courier_verifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    courier_id = table.Column<int>(type: "int", nullable: false),
                    verification_token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expiry_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_used = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courier_verifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_courier_verifications_courier_service_courier_id",
                        column: x => x.courier_id,
                        principalTable: "courier_service",
                        principalColumn: "courier_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "designs",
                columns: table => new
                {
                    design_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    prod_subcat_id = table.Column<int>(type: "int", nullable: false),
                    design_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_designs", x => x.design_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "fabrics",
                columns: table => new
                {
                    fabric_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    prod_subcat_id = table.Column<int>(type: "int", nullable: false),
                    fabric_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fabrics", x => x.fabric_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pending_transporter_verifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    json_payload = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expiry_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pending_transporter_verifications", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "product_sizes",
                columns: table => new
                {
                    size_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    prod_subcat_id = table.Column<int>(type: "int", nullable: false),
                    size_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    length = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    width = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    height = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    weight = table.Column<decimal>(type: "decimal(65,30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_sizes", x => x.size_id);
                    table.ForeignKey(
                        name: "FK_product_sizes_product_sub_categories_prod_subcat_id",
                        column: x => x.prod_subcat_id,
                        principalTable: "product_sub_categories",
                        principalColumn: "prod_subcat_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "product_type",
                columns: table => new
                {
                    prod_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    prod_subcat_id = table.Column<int>(type: "int", nullable: false),
                    prod_type_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_type", x => x.prod_type_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shopper_alternate_address",
                columns: table => new
                {
                    alt_address_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shopper_reg_id = table.Column<int>(type: "int", nullable: false),
                    alt_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_phone_number = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_address = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_town = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_city = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_country = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_postal_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    delivery_notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopper_alternate_address", x => x.alt_address_id);
                    table.ForeignKey(
                        name: "FK_shopper_alternate_address_shopper_registers_shopper_reg_id",
                        column: x => x.shopper_reg_id,
                        principalTable: "shopper_registers",
                        principalColumn: "shopper_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shopper_product_recent_view",
                columns: table => new
                {
                    recent_view_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shopper_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    last_viewed_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    view_count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopper_product_recent_view", x => x.recent_view_id);
                    table.ForeignKey(
                        name: "FK_shopper_product_recent_view_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shopper_product_recent_view_shopper_registers_shopper_id",
                        column: x => x.shopper_id,
                        principalTable: "shopper_registers",
                        principalColumn: "shopper_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "store_orders",
                columns: table => new
                {
                    store_order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    store_id = table.Column<int>(type: "int", nullable: false),
                    courier_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    store_total_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_orders", x => x.store_order_id);
                    table.ForeignKey(
                        name: "FK_store_orders_business_registers_store_id",
                        column: x => x.store_id,
                        principalTable: "business_registers",
                        principalColumn: "bus_reg_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_store_orders_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transporter_registers",
                columns: table => new
                {
                    transporter_reg_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    transporter_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    transporter_email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_email_verified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    password = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    town = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    city = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    state = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    country = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    postal_code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone_number = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    transporter_reg_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transporter_registers", x => x.transporter_reg_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_sessions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    user_type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    session_guid = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ip_address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    device_info = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_sessions", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "wishlist",
                columns: table => new
                {
                    wishlist_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    shopper_reg_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    sku_id = table.Column<int>(type: "int", nullable: false),
                    bus_reg_id = table.Column<int>(type: "int", nullable: false),
                    buscat_id = table.Column<int>(type: "int", nullable: false),
                    prod_subcat_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wishlist", x => x.wishlist_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "courier_branch_service",
                columns: table => new
                {
                    branch_service_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    branch_id = table.Column<int>(type: "int", nullable: false),
                    destinations = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    shipping_mode = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    distance_range = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    weight_range = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    charges = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    estimate_days = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courier_branch_service", x => x.branch_service_id);
                    table.ForeignKey(
                        name: "FK_courier_branch_service_courier_branch_branch_id",
                        column: x => x.branch_id,
                        principalTable: "courier_branch",
                        principalColumn: "branch_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "courier_db_notifications",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    courier_id = table.Column<int>(type: "int", nullable: false),
                    branch_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_read = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courier_db_notifications", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_courier_db_notifications_courier_branch_branch_id",
                        column: x => x.branch_id,
                        principalTable: "courier_branch",
                        principalColumn: "branch_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_courier_db_notifications_courier_service_courier_id",
                        column: x => x.courier_id,
                        principalTable: "courier_service",
                        principalColumn: "courier_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "productsize_measurements",
                columns: table => new
                {
                    measurement_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    size_id = table.Column<int>(type: "int", nullable: false),
                    length = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    height = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    width = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    weight = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    unit = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productsize_measurements", x => x.measurement_id);
                    table.ForeignKey(
                        name: "FK_productsize_measurements_product_sizes_size_id",
                        column: x => x.size_id,
                        principalTable: "product_sizes",
                        principalColumn: "size_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sku_product_variants",
                columns: table => new
                {
                    sku_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    color = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    size_id = table.Column<int>(type: "int", nullable: true),
                    sku_cost = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    discount_price = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    quantity = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    length = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    width = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    height = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    weight = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    discount = table.Column<decimal>(type: "decimal(65,30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sku_product_variants", x => x.sku_id);
                    table.ForeignKey(
                        name: "FK_sku_product_variants_product_sizes_size_id",
                        column: x => x.size_id,
                        principalTable: "product_sizes",
                        principalColumn: "size_id");
                    table.ForeignKey(
                        name: "FK_sku_product_variants_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transporter_verification",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    transporter_id = table.Column<int>(type: "int", nullable: false),
                    verification_token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expiry_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_used = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transporter_verification", x => x.id);
                    table.ForeignKey(
                        name: "FK_transporter_verification_transporter_registers_transporter_id",
                        column: x => x.transporter_id,
                        principalTable: "transporter_registers",
                        principalColumn: "transporter_reg_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "product_images",
                columns: table => new
                {
                    image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    sku_id = table.Column<int>(type: "int", nullable: true),
                    file_name = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_images", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_product_images_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_images_sku_product_variants_sku_id",
                        column: x => x.sku_id,
                        principalTable: "sku_product_variants",
                        principalColumn: "sku_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "business_categories",
                columns: new[] { "bus_cat_id", "business_category_name" },
                values: new object[,]
                {
                    { 1, "products" },
                    { 2, "services" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_products_bus_reg_id",
                table: "products",
                column: "bus_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_design_id",
                table: "products",
                column: "design_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_fabric_id",
                table: "products",
                column: "fabric_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_product_type_id",
                table: "products",
                column: "product_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_selected_alt_address_id",
                table: "orders",
                column: "selected_alt_address_id");

            migrationBuilder.CreateIndex(
                name: "IX_orderdetails_sku_id",
                table: "orderdetails",
                column: "sku_id");

            migrationBuilder.CreateIndex(
                name: "IX_orderdetails_store_order_id",
                table: "orderdetails",
                column: "store_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipping_details_branch_id",
                table: "shipping_details",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_shipping_details_store_order_id",
                table: "shipping_details",
                column: "store_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_business_verifications_bus_reg_id",
                table: "business_verifications",
                column: "bus_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_business_registers_bus_cat_id",
                table: "business_registers",
                column: "bus_cat_id");

            migrationBuilder.CreateIndex(
                name: "IX_business_profiles_bus_reg_id",
                table: "business_profiles",
                column: "bus_reg_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_admin_comments_bus_reg_id",
                table: "admin_comments",
                column: "bus_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_courier_branch_courier_id",
                table: "courier_branch",
                column: "courier_id");

            migrationBuilder.CreateIndex(
                name: "IX_courier_branch_service_branch_id",
                table: "courier_branch_service",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_courier_db_notifications_branch_id",
                table: "courier_db_notifications",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_courier_db_notifications_courier_id",
                table: "courier_db_notifications",
                column: "courier_id");

            migrationBuilder.CreateIndex(
                name: "IX_courier_verifications_courier_id",
                table: "courier_verifications",
                column: "courier_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_images_product_id",
                table: "product_images",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_images_sku_id",
                table: "product_images",
                column: "sku_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_sizes_prod_subcat_id",
                table: "product_sizes",
                column: "prod_subcat_id");

            migrationBuilder.CreateIndex(
                name: "IX_productsize_measurements_size_id",
                table: "productsize_measurements",
                column: "size_id");

            migrationBuilder.CreateIndex(
                name: "IX_shopper_alternate_address_shopper_reg_id",
                table: "shopper_alternate_address",
                column: "shopper_reg_id");

            migrationBuilder.CreateIndex(
                name: "IX_shopper_product_recent_view_product_id",
                table: "shopper_product_recent_view",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_shopper_product_recent_view_shopper_id_product_id",
                table: "shopper_product_recent_view",
                columns: new[] { "shopper_id", "product_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sku_product_variants_product_id",
                table: "sku_product_variants",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_sku_product_variants_size_id",
                table: "sku_product_variants",
                column: "size_id");

            migrationBuilder.CreateIndex(
                name: "IX_store_orders_order_id",
                table: "store_orders",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_store_orders_store_id",
                table: "store_orders",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_transporter_verification_transporter_id",
                table: "transporter_verification",
                column: "transporter_id");

            migrationBuilder.AddForeignKey(
                name: "FK_business_profiles_business_registers_bus_reg_id",
                table: "business_profiles",
                column: "bus_reg_id",
                principalTable: "business_registers",
                principalColumn: "bus_reg_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_business_registers_business_categories_bus_cat_id",
                table: "business_registers",
                column: "bus_cat_id",
                principalTable: "business_categories",
                principalColumn: "bus_cat_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_business_verifications_business_registers_bus_reg_id",
                table: "business_verifications",
                column: "bus_reg_id",
                principalTable: "business_registers",
                principalColumn: "bus_reg_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderdetails_business_registers_store_id",
                table: "orderdetails",
                column: "store_id",
                principalTable: "business_registers",
                principalColumn: "bus_reg_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderdetails_orders_order_id",
                table: "orderdetails",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderdetails_products_product_id",
                table: "orderdetails",
                column: "product_id",
                principalTable: "products",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderdetails_sku_product_variants_sku_id",
                table: "orderdetails",
                column: "sku_id",
                principalTable: "sku_product_variants",
                principalColumn: "sku_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderdetails_store_orders_store_order_id",
                table: "orderdetails",
                column: "store_order_id",
                principalTable: "store_orders",
                principalColumn: "store_order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_shopper_alternate_address_selected_alt_address_id",
                table: "orders",
                column: "selected_alt_address_id",
                principalTable: "shopper_alternate_address",
                principalColumn: "alt_address_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_shopper_registers_shopper_reg_id",
                table: "orders",
                column: "shopper_reg_id",
                principalTable: "shopper_registers",
                principalColumn: "shopper_reg_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_payments_orders_order_id",
                table: "payments",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_business_registers_bus_reg_id",
                table: "products",
                column: "bus_reg_id",
                principalTable: "business_registers",
                principalColumn: "bus_reg_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_designs_design_id",
                table: "products",
                column: "design_id",
                principalTable: "designs",
                principalColumn: "design_id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_fabrics_fabric_id",
                table: "products",
                column: "fabric_id",
                principalTable: "fabrics",
                principalColumn: "fabric_id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_product_type_product_type_id",
                table: "products",
                column: "product_type_id",
                principalTable: "product_type",
                principalColumn: "prod_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_shipping_details_courier_branch_branch_id",
                table: "shipping_details",
                column: "branch_id",
                principalTable: "courier_branch",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_shipping_details_orders_order_id",
                table: "shipping_details",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_shipping_details_store_orders_store_order_id",
                table: "shipping_details",
                column: "store_order_id",
                principalTable: "store_orders",
                principalColumn: "store_order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_shopper_verification_shopper_registers_shopper_id",
                table: "shopper_verification",
                column: "shopper_id",
                principalTable: "shopper_registers",
                principalColumn: "shopper_reg_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_business_profiles_business_registers_bus_reg_id",
                table: "business_profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_business_registers_business_categories_bus_cat_id",
                table: "business_registers");

            migrationBuilder.DropForeignKey(
                name: "FK_business_verifications_business_registers_bus_reg_id",
                table: "business_verifications");

            migrationBuilder.DropForeignKey(
                name: "FK_orderdetails_business_registers_store_id",
                table: "orderdetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orderdetails_orders_order_id",
                table: "orderdetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orderdetails_products_product_id",
                table: "orderdetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orderdetails_sku_product_variants_sku_id",
                table: "orderdetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orderdetails_store_orders_store_order_id",
                table: "orderdetails");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_shopper_alternate_address_selected_alt_address_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_shopper_registers_shopper_reg_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_payments_orders_order_id",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "FK_products_business_registers_bus_reg_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_designs_design_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_fabrics_fabric_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_product_type_product_type_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_shipping_details_courier_branch_branch_id",
                table: "shipping_details");

            migrationBuilder.DropForeignKey(
                name: "FK_shipping_details_orders_order_id",
                table: "shipping_details");

            migrationBuilder.DropForeignKey(
                name: "FK_shipping_details_store_orders_store_order_id",
                table: "shipping_details");

            migrationBuilder.DropForeignKey(
                name: "FK_shopper_verification_shopper_registers_shopper_id",
                table: "shopper_verification");

            migrationBuilder.DropTable(
                name: "admin_comments");

            migrationBuilder.DropTable(
                name: "business_categories");

            migrationBuilder.DropTable(
                name: "business_db_notifications");

            migrationBuilder.DropTable(
                name: "business_services");

            migrationBuilder.DropTable(
                name: "courier_branch_service");

            migrationBuilder.DropTable(
                name: "courier_db_notifications");

            migrationBuilder.DropTable(
                name: "courier_verifications");

            migrationBuilder.DropTable(
                name: "designs");

            migrationBuilder.DropTable(
                name: "fabrics");

            migrationBuilder.DropTable(
                name: "pending_transporter_verifications");

            migrationBuilder.DropTable(
                name: "product_images");

            migrationBuilder.DropTable(
                name: "product_type");

            migrationBuilder.DropTable(
                name: "productsize_measurements");

            migrationBuilder.DropTable(
                name: "shopper_alternate_address");

            migrationBuilder.DropTable(
                name: "shopper_product_recent_view");

            migrationBuilder.DropTable(
                name: "store_orders");

            migrationBuilder.DropTable(
                name: "transporter_verification");

            migrationBuilder.DropTable(
                name: "user_sessions");

            migrationBuilder.DropTable(
                name: "wishlist");

            migrationBuilder.DropTable(
                name: "courier_branch");

            migrationBuilder.DropTable(
                name: "sku_product_variants");

            migrationBuilder.DropTable(
                name: "transporter_registers");

            migrationBuilder.DropTable(
                name: "product_sizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subcategoryimages_busregids",
                table: "subcategoryimages_busregids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_registrations",
                table: "registrations");

            migrationBuilder.DropIndex(
                name: "IX_products_bus_reg_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_design_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_fabric_id",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_product_type_id",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payments",
                table: "payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_orders",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_selected_alt_address_id",
                table: "orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_orderdetails",
                table: "orderdetails");

            migrationBuilder.DropIndex(
                name: "IX_orderdetails_sku_id",
                table: "orderdetails");

            migrationBuilder.DropIndex(
                name: "IX_orderdetails_store_order_id",
                table: "orderdetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_shopper_verification",
                table: "shopper_verification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_shopper_registers",
                table: "shopper_registers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_shipping_details",
                table: "shipping_details");

            migrationBuilder.DropIndex(
                name: "IX_shipping_details_branch_id",
                table: "shipping_details");

            migrationBuilder.DropIndex(
                name: "IX_shipping_details_store_order_id",
                table: "shipping_details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pending_verifications",
                table: "pending_verifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pending_courier_verifications",
                table: "pending_courier_verifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pending_business_verifications",
                table: "pending_business_verifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_password_reset_requests",
                table: "password_reset_requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_courier_service",
                table: "courier_service");

            migrationBuilder.DropPrimaryKey(
                name: "PK_business_verifications",
                table: "business_verifications");

            migrationBuilder.DropIndex(
                name: "IX_business_verifications_bus_reg_id",
                table: "business_verifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_business_registers",
                table: "business_registers");

            migrationBuilder.DropIndex(
                name: "IX_business_registers_bus_cat_id",
                table: "business_registers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_business_profiles",
                table: "business_profiles");

            migrationBuilder.DropIndex(
                name: "IX_business_profiles_bus_reg_id",
                table: "business_profiles");

            migrationBuilder.DropColumn(
                name: "design_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "fabric_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "products");

            migrationBuilder.DropColumn(
                name: "product_status",
                table: "products");

            migrationBuilder.DropColumn(
                name: "product_type_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "supplier_name",
                table: "products");

            migrationBuilder.DropColumn(
                name: "stripe_payment_intent_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "selected_alt_address_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "sku_id",
                table: "orderdetails");

            migrationBuilder.DropColumn(
                name: "store_order_id",
                table: "orderdetails");

            migrationBuilder.DropColumn(
                name: "sku_id",
                table: "addtocart");

            migrationBuilder.DropColumn(
                name: "status",
                table: "shopper_registers");

            migrationBuilder.DropColumn(
                name: "branch_id",
                table: "shipping_details");

            migrationBuilder.DropColumn(
                name: "delivered_date",
                table: "shipping_details");

            migrationBuilder.DropColumn(
                name: "delivery_address",
                table: "shipping_details");

            migrationBuilder.DropColumn(
                name: "delivery_proof_file_name",
                table: "shipping_details");

            migrationBuilder.DropColumn(
                name: "address",
                table: "courier_service");

            migrationBuilder.DropColumn(
                name: "city",
                table: "courier_service");

            migrationBuilder.DropColumn(
                name: "country",
                table: "courier_service");

            migrationBuilder.DropColumn(
                name: "courier_website_name",
                table: "courier_service");

            migrationBuilder.DropColumn(
                name: "is_city",
                table: "courier_service");

            migrationBuilder.DropColumn(
                name: "is_state",
                table: "courier_service");

            migrationBuilder.DropColumn(
                name: "state",
                table: "courier_service");

            migrationBuilder.DropColumn(
                name: "town",
                table: "courier_service");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "business_registers");

            migrationBuilder.DropColumn(
                name: "approved_date",
                table: "business_profiles");

            migrationBuilder.DropColumn(
                name: "business_name",
                table: "business_profiles");

            migrationBuilder.DropColumn(
                name: "logo_path",
                table: "business_profiles");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "subcategoryimages_busregids",
                newName: "subcategoryimages_Busregids");

            migrationBuilder.RenameTable(
                name: "registrations",
                newName: "Registrations");

            migrationBuilder.RenameTable(
                name: "payments",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "orders",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "orderdetails",
                newName: "OrderDetails");

            migrationBuilder.RenameTable(
                name: "shopper_verification",
                newName: "ShopperVerification");

            migrationBuilder.RenameTable(
                name: "shopper_registers",
                newName: "ShopperRegisters");

            migrationBuilder.RenameTable(
                name: "shipping_details",
                newName: "ShippingDetails");

            migrationBuilder.RenameTable(
                name: "pending_verifications",
                newName: "PendingVerifications");

            migrationBuilder.RenameTable(
                name: "pending_courier_verifications",
                newName: "PendingCourierVerifications");

            migrationBuilder.RenameTable(
                name: "pending_business_verifications",
                newName: "PendingBusinessVerifications");

            migrationBuilder.RenameTable(
                name: "password_reset_requests",
                newName: "PasswordResetRequests");

            migrationBuilder.RenameTable(
                name: "courier_service",
                newName: "CourierService");

            migrationBuilder.RenameTable(
                name: "business_verifications",
                newName: "BusinessVerifications");

            migrationBuilder.RenameTable(
                name: "business_registers",
                newName: "BusinessRegisters");

            migrationBuilder.RenameTable(
                name: "business_profiles",
                newName: "BusinessProfiles");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "Users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "prod_subcat_name",
                table: "subcategoryimages_Busregids",
                newName: "Prod_subcat_name");

            migrationBuilder.RenameColumn(
                name: "prod_subcat_image",
                table: "subcategoryimages_Busregids",
                newName: "Prod_subcat_image");

            migrationBuilder.RenameColumn(
                name: "prod_subcat_id",
                table: "subcategoryimages_Busregids",
                newName: "Prod_subcat_id");

            migrationBuilder.RenameColumn(
                name: "image_id",
                table: "subcategoryimages_Busregids",
                newName: "Image_Id");

            migrationBuilder.RenameColumn(
                name: "buscat_id",
                table: "subcategoryimages_Busregids",
                newName: "BuscatId");

            migrationBuilder.RenameColumn(
                name: "bus_reg_id",
                table: "subcategoryimages_Busregids",
                newName: "BusRegId");

            migrationBuilder.RenameColumn(
                name: "busserv_id",
                table: "services_sub_categories",
                newName: "BusservId");

            migrationBuilder.RenameColumn(
                name: "bus_serv_id",
                table: "services",
                newName: "BusservId");

            migrationBuilder.RenameColumn(
                name: "bus_reg_id",
                table: "services",
                newName: "BusRegId");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Registrations",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "town",
                table: "Registrations",
                newName: "Town");

            migrationBuilder.RenameColumn(
                name: "state",
                table: "Registrations",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "Registrations",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "otp",
                table: "Registrations",
                newName: "Otp");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Registrations",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "dob",
                table: "Registrations",
                newName: "Dob");

            migrationBuilder.RenameColumn(
                name: "country",
                table: "Registrations",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "Registrations",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "phone_no",
                table: "Registrations",
                newName: "PhoneNo");

            migrationBuilder.RenameColumn(
                name: "new_password",
                table: "Registrations",
                newName: "NewPassword");

            migrationBuilder.RenameColumn(
                name: "cnf_password",
                table: "Registrations",
                newName: "CnfPassword");

            migrationBuilder.RenameColumn(
                name: "reg_id",
                table: "Registrations",
                newName: "RegId");

            migrationBuilder.RenameColumn(
                name: "buscat_id",
                table: "products",
                newName: "BuscatId");

            migrationBuilder.RenameColumn(
                name: "bus_reg_id",
                table: "products",
                newName: "BusRegId");

            migrationBuilder.RenameColumn(
                name: "bus_cat_id",
                table: "product_sub_categories",
                newName: "BuscatId");

            migrationBuilder.RenameColumn(
                name: "payment_status",
                table: "Payments",
                newName: "PaymentStatus");

            migrationBuilder.RenameColumn(
                name: "payment_method",
                table: "Payments",
                newName: "PaymentMethod");

            migrationBuilder.RenameColumn(
                name: "payment_date",
                table: "Payments",
                newName: "PaymentDate");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "Payments",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "amount_paid",
                table: "Payments",
                newName: "AmountPaid");

            migrationBuilder.RenameColumn(
                name: "payment_id",
                table: "Payments",
                newName: "PaymentId");

            migrationBuilder.RenameIndex(
                name: "IX_payments_order_id",
                table: "Payments",
                newName: "IX_Payments_OrderId");

            migrationBuilder.RenameColumn(
                name: "orderstatus",
                table: "Orders",
                newName: "OrderStatus");

            migrationBuilder.RenameColumn(
                name: "total_amount",
                table: "Orders",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "shopper_reg_id",
                table: "Orders",
                newName: "ShopperRegId");

            migrationBuilder.RenameColumn(
                name: "shipping_type",
                table: "Orders",
                newName: "ShippingType");

            migrationBuilder.RenameColumn(
                name: "order_date",
                table: "Orders",
                newName: "OrderDate");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "Orders",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_orders_shopper_reg_id",
                table: "Orders",
                newName: "IX_Orders_ShopperRegId");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "OrderDetails",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "OrderDetails",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "store_id",
                table: "OrderDetails",
                newName: "StoreId");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "OrderDetails",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "OrderDetails",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "order_detail_id",
                table: "OrderDetails",
                newName: "OrderDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_orderdetails_store_id",
                table: "OrderDetails",
                newName: "IX_OrderDetails_StoreId");

            migrationBuilder.RenameIndex(
                name: "IX_orderdetails_product_id",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_orderdetails_order_id",
                table: "OrderDetails",
                newName: "IX_OrderDetails_OrderId");

            migrationBuilder.RenameColumn(
                name: "shopper_reg_id",
                table: "addtocart",
                newName: "ShopperRegId");

            migrationBuilder.RenameColumn(
                name: "buscat_id",
                table: "addtocart",
                newName: "BuscatId");

            migrationBuilder.RenameColumn(
                name: "bus_reg_id",
                table: "addtocart",
                newName: "BusRegId");

            migrationBuilder.RenameColumn(
                name: "cart_id",
                table: "addtocart",
                newName: "CartId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ShopperVerification",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "verification_token",
                table: "ShopperVerification",
                newName: "VerificationToken");

            migrationBuilder.RenameColumn(
                name: "shopper_id",
                table: "ShopperVerification",
                newName: "ShopperId");

            migrationBuilder.RenameColumn(
                name: "is_used",
                table: "ShopperVerification",
                newName: "IsUsed");

            migrationBuilder.RenameColumn(
                name: "expiry_date",
                table: "ShopperVerification",
                newName: "ExpiryDate");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ShopperVerification",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_shopper_verification_shopper_id",
                table: "ShopperVerification",
                newName: "IX_ShopperVerification_ShopperId");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "ShopperRegisters",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "town",
                table: "ShopperRegisters",
                newName: "Town");

            migrationBuilder.RenameColumn(
                name: "state",
                table: "ShopperRegisters",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "ShopperRegisters",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "ShopperRegisters",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "country",
                table: "ShopperRegisters",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "ShopperRegisters",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "ShopperRegisters",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "shopper_reg_date",
                table: "ShopperRegisters",
                newName: "ShopperRegDate");

            migrationBuilder.RenameColumn(
                name: "postal_code",
                table: "ShopperRegisters",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "photo_name",
                table: "ShopperRegisters",
                newName: "PhotoName");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "ShopperRegisters",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "is_email_verified",
                table: "ShopperRegisters",
                newName: "IsEmailVerified");

            migrationBuilder.RenameColumn(
                name: "shopper_reg_id",
                table: "ShopperRegisters",
                newName: "ShopperRegId");

            migrationBuilder.RenameColumn(
                name: "shipping_type",
                table: "ShippingDetails",
                newName: "Shipping_type");

            migrationBuilder.RenameColumn(
                name: "cost",
                table: "ShippingDetails",
                newName: "Cost");

            migrationBuilder.RenameColumn(
                name: "tracking_id",
                table: "ShippingDetails",
                newName: "TrackingId");

            migrationBuilder.RenameColumn(
                name: "shipping_status",
                table: "ShippingDetails",
                newName: "ShippingStatus");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "ShippingDetails",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "estimated_days",
                table: "ShippingDetails",
                newName: "EstimatedDays");

            migrationBuilder.RenameColumn(
                name: "shipping_detail_id",
                table: "ShippingDetails",
                newName: "ShippingDetailId");

            migrationBuilder.RenameColumn(
                name: "store_order_id",
                table: "ShippingDetails",
                newName: "OrderDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_shipping_details_order_id",
                table: "ShippingDetails",
                newName: "IX_ShippingDetails_OrderId");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "PendingVerifications",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "PendingVerifications",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PendingVerifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "json_payload",
                table: "PendingVerifications",
                newName: "JsonPayload");

            migrationBuilder.RenameColumn(
                name: "expiry_date",
                table: "PendingVerifications",
                newName: "ExpiryDate");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "PendingCourierVerifications",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "PendingCourierVerifications",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PendingCourierVerifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "json_payload",
                table: "PendingCourierVerifications",
                newName: "JsonPayload");

            migrationBuilder.RenameColumn(
                name: "expiry_date",
                table: "PendingCourierVerifications",
                newName: "ExpiryDate");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "PendingBusinessVerifications",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "PendingBusinessVerifications",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PendingBusinessVerifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "json_payload",
                table: "PendingBusinessVerifications",
                newName: "JsonPayload");

            migrationBuilder.RenameColumn(
                name: "expiry_date",
                table: "PendingBusinessVerifications",
                newName: "ExpiryDate");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "PasswordResetRequests",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "expiry",
                table: "PasswordResetRequests",
                newName: "Expiry");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "PasswordResetRequests",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PasswordResetRequests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "CourierService",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "registered_date",
                table: "CourierService",
                newName: "RegisteredDate");

            migrationBuilder.RenameColumn(
                name: "postal_code",
                table: "CourierService",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "is_email_verified",
                table: "CourierService",
                newName: "IsEmailVerified");

            migrationBuilder.RenameColumn(
                name: "courier_service_name",
                table: "CourierService",
                newName: "CourierServiceName");

            migrationBuilder.RenameColumn(
                name: "courier_phone",
                table: "CourierService",
                newName: "CourierPhone");

            migrationBuilder.RenameColumn(
                name: "courier_email",
                table: "CourierService",
                newName: "CourierEmail");

            migrationBuilder.RenameColumn(
                name: "courier_id",
                table: "CourierService",
                newName: "CourierId");

            migrationBuilder.RenameColumn(
                name: "profile_status",
                table: "CourierService",
                newName: "LicenseNumber");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "BusinessVerifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "verification_token",
                table: "BusinessVerifications",
                newName: "VerificationToken");

            migrationBuilder.RenameColumn(
                name: "is_used",
                table: "BusinessVerifications",
                newName: "IsUsed");

            migrationBuilder.RenameColumn(
                name: "expiry_date",
                table: "BusinessVerifications",
                newName: "ExpiryDate");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "BusinessVerifications",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "bus_reg_id",
                table: "BusinessVerifications",
                newName: "BusRegId");

            migrationBuilder.RenameColumn(
                name: "town",
                table: "BusinessRegisters",
                newName: "Town");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "BusinessRegisters",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "gstin",
                table: "BusinessRegisters",
                newName: "Gstin");

            migrationBuilder.RenameColumn(
                name: "postal_code",
                table: "BusinessRegisters",
                newName: "postalCode");

            migrationBuilder.RenameColumn(
                name: "license_type",
                table: "BusinessRegisters",
                newName: "LicenseType");

            migrationBuilder.RenameColumn(
                name: "is_email_verified",
                table: "BusinessRegisters",
                newName: "IsEmailVerified");

            migrationBuilder.RenameColumn(
                name: "business_username",
                table: "BusinessRegisters",
                newName: "BusinessUsername");

            migrationBuilder.RenameColumn(
                name: "business_state",
                table: "BusinessRegisters",
                newName: "businessState");

            migrationBuilder.RenameColumn(
                name: "business_reg_date",
                table: "BusinessRegisters",
                newName: "BusinessRegDate");

            migrationBuilder.RenameColumn(
                name: "business_name",
                table: "BusinessRegisters",
                newName: "Businessname");

            migrationBuilder.RenameColumn(
                name: "business_country",
                table: "BusinessRegisters",
                newName: "businessCountry");

            migrationBuilder.RenameColumn(
                name: "business_city",
                table: "BusinessRegisters",
                newName: "businessCity");

            migrationBuilder.RenameColumn(
                name: "bus_serv_id",
                table: "BusinessRegisters",
                newName: "BusservId");

            migrationBuilder.RenameColumn(
                name: "bus_mobile_no",
                table: "BusinessRegisters",
                newName: "BusMobileNo");

            migrationBuilder.RenameColumn(
                name: "bus_email",
                table: "BusinessRegisters",
                newName: "BusEmail");

            migrationBuilder.RenameColumn(
                name: "bus_cat_id",
                table: "BusinessRegisters",
                newName: "BuscatId");

            migrationBuilder.RenameColumn(
                name: "address_2",
                table: "BusinessRegisters",
                newName: "Address2");

            migrationBuilder.RenameColumn(
                name: "address_1",
                table: "BusinessRegisters",
                newName: "Address1");

            migrationBuilder.RenameColumn(
                name: "bus_reg_id",
                table: "BusinessRegisters",
                newName: "BusRegId");

            migrationBuilder.RenameColumn(
                name: "bus_serv_id",
                table: "BusinessProfiles",
                newName: "BusServId");

            migrationBuilder.RenameColumn(
                name: "bus_reg_id",
                table: "BusinessProfiles",
                newName: "BusRegId");

            migrationBuilder.RenameColumn(
                name: "bus_cat_id",
                table: "BusinessProfiles",
                newName: "BusCatId");

            migrationBuilder.RenameColumn(
                name: "business_profile_id",
                table: "BusinessProfiles",
                newName: "businessprofile_id");

            migrationBuilder.AlterColumn<decimal>(
                name: "service_cost",
                table: "services",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_subject",
                keyValue: null,
                column: "product_subject",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "product_subject",
                table: "products",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "product_image",
                keyValue: null,
                column: "product_image",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "product_image",
                table: "products",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "product_cost",
                table: "products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "product_height",
                table: "products",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "product_length",
                table: "products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "product_quantity",
                table: "products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "product_weight",
                table: "products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "product_width",
                table: "products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "product_sub_categories",
                keyColumn: "prod_subcat_image",
                keyValue: null,
                column: "prod_subcat_image",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "prod_subcat_image",
                table: "product_sub_categories",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Shipping_type",
                table: "ShippingDetails",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "ShippingDetails",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingId",
                table: "ShippingDetails",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ShippingStatus",
                table: "ShippingDetails",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "CourierService",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "CourierService",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CourierServiceName",
                table: "CourierService",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AadharNumber",
                table: "CourierService",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CourierAddress",
                table: "CourierService",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CourierCity",
                table: "CourierService",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CourierCountry",
                table: "CourierService",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CourierState",
                table: "CourierService",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CourierTown",
                table: "CourierService",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "BusinessBusRegId",
                table: "BusinessVerifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "BusinessRegisters",
                keyColumn: "Address2",
                keyValue: null,
                column: "Address2",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Address2",
                table: "BusinessRegisters",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "BusinessProfiles",
                keyColumn: "profile_status",
                keyValue: null,
                column: "profile_status",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "profile_status",
                table: "BusinessProfiles",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "BusinessProfiles",
                keyColumn: "business_location",
                keyValue: null,
                column: "business_location",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "business_location",
                table: "BusinessProfiles",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "BusinessProfiles",
                keyColumn: "business_about",
                keyValue: null,
                column: "business_about",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "business_about",
                table: "BusinessProfiles",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "BusinessProfiles",
                keyColumn: "banner_path",
                keyValue: null,
                column: "banner_path",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "banner_path",
                table: "BusinessProfiles",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BusinessUsername",
                table: "BusinessProfiles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Businesscategory_name",
                table: "BusinessProfiles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Businessservice_name",
                table: "BusinessProfiles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "bus_time",
                table: "BusinessProfiles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "image_positionx",
                table: "BusinessProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "image_positiony",
                table: "BusinessProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "zoom",
                table: "BusinessProfiles",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subcategoryimages_Busregids",
                table: "subcategoryimages_Busregids",
                column: "Image_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Registrations",
                table: "Registrations",
                column: "RegId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "PaymentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDetails",
                table: "OrderDetails",
                column: "OrderDetailId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopperVerification",
                table: "ShopperVerification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopperRegisters",
                table: "ShopperRegisters",
                column: "ShopperRegId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShippingDetails",
                table: "ShippingDetails",
                column: "ShippingDetailId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PendingVerifications",
                table: "PendingVerifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PendingCourierVerifications",
                table: "PendingCourierVerifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PendingBusinessVerifications",
                table: "PendingBusinessVerifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PasswordResetRequests",
                table: "PasswordResetRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourierService",
                table: "CourierService",
                column: "CourierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessVerifications",
                table: "BusinessVerifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessRegisters",
                table: "BusinessRegisters",
                column: "BusRegId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessProfiles",
                table: "BusinessProfiles",
                column: "businessprofile_id");

            migrationBuilder.CreateTable(
                name: "BusinessCategories",
                columns: table => new
                {
                    BuscatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Businesscategory_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessCategories", x => x.BuscatId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BusinessServices",
                columns: table => new
                {
                    BusservId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Businessservice_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessServices", x => x.BusservId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "BusinessCategories",
                columns: new[] { "BuscatId", "Businesscategory_name" },
                values: new object[,]
                {
                    { 1, "products" },
                    { 2, "services" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessVerifications_BusinessBusRegId",
                table: "BusinessVerifications",
                column: "BusinessBusRegId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessVerifications_BusinessRegisters_BusinessBusRegId",
                table: "BusinessVerifications",
                column: "BusinessBusRegId",
                principalTable: "BusinessRegisters",
                principalColumn: "BusRegId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_BusinessRegisters_StoreId",
                table: "OrderDetails",
                column: "StoreId",
                principalTable: "BusinessRegisters",
                principalColumn: "BusRegId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ShopperRegisters_ShopperRegId",
                table: "Orders",
                column: "ShopperRegId",
                principalTable: "ShopperRegisters",
                principalColumn: "ShopperRegId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingDetails_Orders_OrderId",
                table: "ShippingDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopperVerification_ShopperRegisters_ShopperId",
                table: "ShopperVerification",
                column: "ShopperId",
                principalTable: "ShopperRegisters",
                principalColumn: "ShopperRegId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
