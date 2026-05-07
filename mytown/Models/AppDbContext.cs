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
            public DbSet<Registration> Registrations { get; set; } // Pluralized name
            public DbSet<BusinessRegister> BusinessRegisters { get; set; } // done
           public DbSet<BusinessVerification> BusinessVerification { get; set; }//done
            public DbSet<BusinessProfile> BusinessProfiles { get; set; }//done
            public DbSet<BusinessService> BusinessServices { get; set; } //done
            public DbSet<BusinessCategory> BusinessCategories { get; set; }//done
            public DbSet<ProductSubCategory> product_sub_categories { get; set; } //done
            public DbSet<services_sub_categories> services_sub_categories { get; set; }
            public DbSet<Products> products { get; set; } //done
            public DbSet<Service> services { get; set; }
            public DbSet<ShopperRegister> ShopperRegisters { get; set; } //done
           public DbSet<ShopperVerification> ShopperVerification { get; set; } //done
            public DbSet<subcategoryimages_busregid> Subcategoryimages_Busregids { get; set; }
            public IEnumerable<object> businessprofile { get; internal set; }

            public DbSet<AddToCart> addtocart { get; set; } //done
            public DbSet<Order> Orders { get; set; } //done

            public DbSet<orderdetails> OrderDetails { get; set; } //done
            public DbSet<Payments> Payments { get; set; } //done
            public DbSet<PendingVerification> PendingVerifications{ get; set; }
            public DbSet<PendingBusinessVerification> PendingBusinessVerifications { get; set; }//done
            public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; } //done
            public DbSet<ShippingDetails> ShippingDetails { get; set; } //done
            public DbSet<CourierService> CourierService { get; set; } //DONE
            public DbSet<PendingCourierVerification> PendingCourierVerifications { get; set; }

            public DbSet<CourierVerification> CourierVerifications { get; set; } //new
            public DbSet<CourierBranch> CourierBranches { get; set; } //modified table

           
            public DbSet<ShopperProductRecentView> ShopperProductRecentViews { get; set; } //done
            public DbSet<ProductImage> ProductImages { get; set; } //done

            public DbSet<ProductType> Product_Types { get; set; } //done
            public DbSet<Fabric> Fabrics { get; set; } //done
            public DbSet<Design> Designs { get; set; } //done
            public DbSet<ProductSize> ProductSizes { get; set; } //done
            public DbSet<Sku_ProductVariant> Sku_ProductVariants { get; set; } //done
            //  public DbSet<ProductImage> ProductImages { get; set; }

            public DbSet<ProductSize_Measurement> ProductSize_Measurements { get; set; }

            public DbSet<ShopperAlternateAddress> ShopperAlternateAddresses { get; set; } //done

            public DbSet<AdminComment> AdminComments { get; set; } //done

            public DbSet<UserSession> UserSessions { get; set; }

            public DbSet<StoreOrder> StoreOrders { get; set; } //done

            public DbSet<BusinessDBNotifications> BusinessDBNotifications { get; set; }
            public DbSet<CourierDBNotifications> CourierDBNotifications { get; set; }

            public DbSet<ShopperDBNotifications> ShopperDBNotifications { get; set; }

            public DbSet<CourierBranchService> CourierBranchServices { get; set; } // latest table

            public DbSet<Wishlist> Wishlist { get; set; }// new

            public DbSet<TransporterRegister> TransporterRegisters { get; set; } //new

            public DbSet<PendingTransporterVerification>PendingTransporterVerifications { get; set; }
            public DbSet<TransporterVerification> TransporterVerification { get; set; }

            public DbSet<TransporterKYC> TransporterKYCs { get; set; }
            public DbSet<TransporterBankDetails> TransporterBankDetails { get; set; }
            public DbSet<TransporterTravelPlan> TransporterTravelPlans { get; set; }
            public DbSet<TransporterDeliveryRequest> TransporterDeliveryRequests { get; set; }
            public DbSet<TransporterExceptionReport> TransporterExceptionReports { get; set; }
            public DbSet<TransporterDBNotifications> TransporterDBNotifications { get; set; }

            //sender
            public DbSet<SenderRegister> SenderRegisters { get; set; }
            public DbSet<PendingSenderVerification> PendingSenderVerifications { get; set; }
            public DbSet<SenderVerification> SenderVerifications { get; set; }

            //package dimensions
            public DbSet<ShippingPackageDetails> ShippingPackageDetails { get; set; }

            //Senderorder
            public DbSet<SenderOrder> SenderOrders { get; set; }
             public DbSet<SenderOrderPayment> SenderOrderPayments { get; set; }









            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

               

                // Optionally, you can add similar configuration for other models
                modelBuilder.Entity<Service>(entity =>
                {
                    entity.Property(e => e.ServiceCost)
                          .HasPrecision(18, 2); // Define precision for service_cost
                });
                modelBuilder.Entity<subcategoryimages_busregid>().ToTable("subcategoryimages_Busregids");

                // Seed data for businesscategoriescs: inserting "products" and "services"
                modelBuilder.Entity<BusinessCategory>().HasData(
                    new BusinessCategory { BusCatId = 1, BusinessCategoryName = "products" },
                    new BusinessCategory { BusCatId = 2, BusinessCategoryName = "services" }
                );

                modelBuilder.Entity<ShopperProductRecentView>()
                   .HasIndex(v => new { v.ShopperId, v.ProductId })
                   .IsUnique();

                modelBuilder.Entity<ProductImage>()
             .HasOne(pi => pi.Product)
             .WithMany(p => p.Images)   // you need to add ICollection<ProductImage> Images in products model
             .HasForeignKey(pi => pi.ProductId)
             .OnDelete(DeleteBehavior.Cascade);

                // Convert all table and column names to snake_case
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

