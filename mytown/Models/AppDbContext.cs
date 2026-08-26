using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.DTO_s;
using MyTown.Models;
using System.Text.RegularExpressions;
using static mytown.Models.BusinessDBNotifications;


namespace mytown.Models
{
    namespace mytown.DataAccess
    {
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            public DbSet<User> Users { get; set; }
            public DbSet<Registration> Registrations { get; set; }
            public DbSet<BusinessRegister> BusinessRegisters { get; set; }
            public DbSet<BusinessVerification> BusinessVerification { get; set; }
            public DbSet<BusinessProfile> BusinessProfiles { get; set; }
            public DbSet<BusinessService> BusinessServices { get; set; }
            public DbSet<BusinessCategory> BusinessCategories { get; set; }
            public DbSet<ProductSubCategory> product_sub_categories { get; set; }

            //bank account details
            public DbSet<BusinessAccountDetail> BusinessAccountDetails { get; set; }
            public DbSet<CourierAccountDetail> CourierAccountDetails { get; set; }

            // services 
            public DbSet<ServiceSubCategory> ServiceSubCategory { get; set; }
            public DbSet<Service> Service { get; set; }
            public DbSet<ServiceProfile> ServiceProfiles { get; set; }
            public DbSet<Products> products { get; set; }

            public DbSet<ShopperRegister> ShopperRegisters { get; set; }
            public DbSet<ShopperVerification> ShopperVerification { get; set; }
            public DbSet<subcategoryimages_busregid> Subcategoryimages_Busregids { get; set; }
            public IEnumerable<object> businessprofile { get; internal set; }

            public DbSet<AddToCart> addtocart { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<orderdetails> OrderDetails { get; set; }
            public DbSet<Payments> Payments { get; set; }
            public DbSet<PendingVerification> PendingVerifications { get; set; }
            public DbSet<PendingBusinessVerification> PendingBusinessVerifications { get; set; }
            public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
            public DbSet<ShippingDetails> ShippingDetails { get; set; }
            public DbSet<CourierService> CourierService { get; set; }
            public DbSet<PendingCourierVerification> PendingCourierVerifications { get; set; }
            public DbSet<PendingGuestVerification> PendingGuestVerifications { get; set; }
            public DbSet<CourierVerification> CourierVerifications { get; set; }
            public DbSet<CourierBranch> CourierBranches { get; set; }

            public DbSet<ShopperProductRecentView> ShopperProductRecentViews { get; set; }
            public DbSet<ProductImage> ProductImages { get; set; }
            public DbSet<ProductType> Product_Types { get; set; }
            public DbSet<ProductGroup> Product_Groups { get; set; }
            public DbSet<ProductAttributes> ProductAttributes { get; set; }
            public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
            public DbSet<Fabric> Fabrics { get; set; }
            public DbSet<Design> Designs { get; set; }
            public DbSet<ProductSize> ProductSizes { get; set; }
            public DbSet<Sku_ProductVariant> Sku_ProductVariants { get; set; }
            public DbSet<ProductSize_Measurement> ProductSize_Measurements { get; set; }
            public DbSet<ShopperAlternateAddress> ShopperAlternateAddresses { get; set; }
            public DbSet<AdminComment> AdminComments { get; set; }
            public DbSet<UserSession> UserSessions { get; set; }
            public DbSet<StoreOrder> StoreOrders { get; set; }

            public DbSet<BusinessDBNotifications> BusinessDBNotifications { get; set; }
            public DbSet<CourierDBNotifications> CourierDBNotifications { get; set; }
            public DbSet<ShopperDBNotifications> ShopperDBNotifications { get; set; }

            public DbSet<CourierBranchService> CourierBranchServices { get; set; }
            public DbSet<Wishlist> Wishlist { get; set; }
            public DbSet<GuestRegister> GuestRegisters { get; set; }

            public DbSet<TransporterRegister> TransporterRegisters { get; set; }
            public DbSet<PendingTransporterVerification> PendingTransporterVerifications { get; set; }
            public DbSet<TransporterVerification> TransporterVerification { get; set; }
            public DbSet<TransporterKYC> TransporterKYCs { get; set; }
            public DbSet<TransporterBankDetails> TransporterBankDetails { get; set; }
            public DbSet<TransporterTravelPlan> TransporterTravelPlans { get; set; }
            public DbSet<TransporterDeliveryRequest> TransporterDeliveryRequests { get; set; }
            public DbSet<TransporterExceptionReport> TransporterExceptionReports { get; set; }
            public DbSet<TransporterDBNotifications> TransporterDBNotifications { get; set; }

            // sender
            public DbSet<SenderRegister> SenderRegisters { get; set; }
            public DbSet<PendingSenderVerification> PendingSenderVerifications { get; set; }
            public DbSet<SenderVerification> SenderVerifications { get; set; }

            // package dimensions
            public DbSet<ShippingPackageDetails> ShippingPackageDetails { get; set; }

            // sender order
            public DbSet<SenderOrder> SenderOrders { get; set; }
            public DbSet<SenderOrderPayment> SenderOrderPayments { get; set; }
            public DbSet<SenderDBNotifications> SenderDBNotifications { get; set; }
            public DbSet<SenderAlternateAddress> SenderAlternateAddresses { get; set; }

            // shopper experience
            public DbSet<ShopperExperience> ShopperExperiences { get; set; }
            public DbSet<ShopperExperiencePhoto> ShopperExperiencePhotos { get; set; }
            public DbSet<BusinessProfileViewer> BusinessProfileViewers { get; set; }
            public DbSet<BusinessConnection> BusinessConnections { get; set; }

            // location images
            public DbSet<LocationImage> LocationImages { get; set; }
            public DbSet<CityImage> CityImages { get; set; }

            //new product form

            public DbSet<ProductsNew> ProductsNew { get; set; }

            public DbSet<ProductVariantNew> ProductVariantsNew { get; set; }

            public DbSet<ProductVariantAttributeNew> ProductVariantAttributesNew { get; set; }
            public DbSet<ProductVariantImageNew> ProductVariantImagesNew { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<subcategoryimages_busregid>().ToTable("subcategoryimages_Busregids");

                modelBuilder.Entity<BusinessCategory>().HasData(
                    new BusinessCategory { BusCatId = 1, BusinessCategoryName = "products" },
                    new BusinessCategory { BusCatId = 2, BusinessCategoryName = "services" }
                );

                modelBuilder.Entity<ShopperProductRecentView>()
                    .HasIndex(v => new { v.ShopperId, v.ProductId })
                    .IsUnique();

                modelBuilder.Entity<ProductImage>()
                    .HasOne(pi => pi.Product)
                    .WithMany(p => p.Images)
                    .HasForeignKey(pi => pi.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                {
                    entity.SetTableName(ToSnakeCase(entity.GetTableName()));

                    foreach (var property in entity.GetProperties())
                    {
                        property.SetColumnName(ToSnakeCase(property.GetColumnName()));
                    }
                }
            }

            private static string ToSnakeCase(string name)
            {
                if (string.IsNullOrEmpty(name)) return name;
                return Regex.Replace(name, "([a-z0-9])([A-Z])", "$1_$2").ToLower();
            }
        }
    }
}