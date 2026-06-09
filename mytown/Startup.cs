using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mytown.Controllers;
using mytown.Controllers.Helpers;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Models;
using mytown.Models.mytown.DataAccess;
using Stripe.Climate;
using System.Security.Claims;
using System.Text;
using mytown.Services.Interfaces;
using mytown.Services.Implementations;
using mytown.DataAccess.Implementations;
using Stripe;



public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // Main entry for service registration; this method calls several private helpers.
    public void ConfigureServices(IServiceCollection services)
    {
        RegisterDatabase(services);
        RegisterApplicationServices(services);
        RegisterControllersAndSwagger(services);
        RegisterCors(services);
        RegisterAuthentication(services);
        services.AddMemoryCache();
    }

    // Registers the database (EF Core with MySQL).
    private void RegisterDatabase(IServiceCollection services)
    {
        var connectionString = Configuration.GetConnectionString("mysqlConnection");
        Console.WriteLine($"EF Core Connection String: {connectionString}");
        services.AddDbContext<AppDbContext>(options =>
           options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    }

    // Consolidates all AddScoped registrations.
private void RegisterApplicationServices(IServiceCollection services)
{
    StripeConfiguration.ApiKey = Configuration["Stripe:SecretKey"];
    services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IShopperRepository, ShopperRepository>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IShopperRegistrationValidator, ShopperRegistrationValidator>();
        services.AddScoped<IVerificationLinkBuilder, VerificationLinkBuilder>();
        services.AddScoped<IVerificationLinkBuilderbusiness,VerificationLinkBuilderbusiness>();
        services.AddScoped<mytown.DataAccess.IBusinessRepository, BusinessRepository>();
        services.AddScoped<IBusinessRegistrationValidator, BusinessRegistrationValidator>();
        services.AddScoped<IVerificationLinkBuildertransporter, VerificationLinkBuildertransporter>();
       
        services.AddScoped<IVerificationLinkBuilderGuest, VerificationLinkBuilderGuest>();
      
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IBusinessProfileRepository, BusinessProfileRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBusinessDashboardRepository, BusinessDashboardRepository>();
        services.AddScoped<ICourierServiceRepository, CourierServiceRepository>();
        services.AddScoped<IVerificationLinkBuildercourier, VerificationLinkBuildercourier>();
        services.AddScoped<ISearchRepository, SearchRepository>();
        services.AddScoped<IShopperDashboardRepository, ShopperDashboardRepository>();
        services.AddScoped<ICourierDashboardRepository, CourierDashboardRepository>(); //latest
        services.AddScoped<ITransporterRepository, TransporterRepository>();
        services.AddScoped<ITransporterDashboardRepository, TransporterDashboardRepository>();
        services.AddScoped<ISenderRepository, SenderRepository>();


        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IFileService, mytown.Services.FileService>();
        services.AddScoped<IUserService, mytown.Services.UserService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IBusinessService, mytown.Services.BusinessService>();

     //  services.AddScoped<IBusinessService, mytown.Services.BusinessService>();

        services.AddScoped<IBusinessProfileService, BusinessProfileService>();
        services.AddScoped<IShopperService, ShopperService>();
        services.AddScoped<IProductService, mytown.Services.Implementations.ProductService>();
        services.AddScoped<IOrderService, mytown.Services.Implementations.OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ISearchService, mytown.Services.SearchService>();
        services.AddScoped<ICourierServiceHandler, mytown.Services.Implementations.CourierServiceHandler>();
        services.AddScoped<IBusinessDashboardService, BusinessDashboardService>();
        services.AddScoped<IShopperDashboardService, ShopperDashboardService>();
        services.AddScoped<ICourierDashboardService, CourierDashboardService>();
        services.AddScoped<ITransporterService, TransporterService>();
        services.AddScoped<ISenderService, SenderService>();
        services.AddScoped<IVerficationLinkBuildersender, VerificationLinkBuildersender>();
        services.AddScoped<ITransporterDashboardService, TransporterDashboardService>();
        services.AddScoped<IBusinessServiceRepository, BusinessServiceRepository>();
        services.AddScoped<IServicesProfile, ServicesProfile>();
        services.AddScoped<IServiceSubCategoryRepository, ServiceSubCategoryRepository>();
        services.AddScoped<IServiceSubCategoryService, ServiceSubCategoryService>();
        services.AddScoped<IGuestRepository, GuestRepository>();
        services.AddScoped<IGuestService, GuestService>();

    }

    // Registers controllers and Swagger (for API documentation).
    private void RegisterControllersAndSwagger(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "MyTown API",
                Version = "v1"
            });

            //  Add JWT Auth to Swagger
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "Enter JWT token like: **Bearer your_token_here**",
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
        });
    }


    // Configures the CORS policy.
    private void RegisterCors(IServiceCollection services)
    {
        var allowedOrigins = new List<string>
        {
            "http://localhost:3000", // Local frontend
            "http://localhost:3001",
            "https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net" ,// Production frontend
            "https://mytown-webapp-gzcyexgdhmgfdzf2.centralindia-01.azurewebsites.net", // new webapp service
                "https://kind-meadow-0fe6b9000.7.azurestaticapps.net", // new static web app for frontend
                "https://www.itismytown.com",
                "https://mytown-webapp-staging-erd7ekb9d9g8bvfk.centralindia-01.azurewebsites.net", // staging webapp service
                "https://jolly-sea-066e8b500.7.azurestaticapps.net",// staging static web app for frontend
                "https://kind-meadow-0fe6b9000-qa.eastasia.7.azurestaticapps.net" // staging slot new static web app for frontend
        };
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(allowedOrigins.ToArray())
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
    }

    // Configures JWT Bearer authentication.
    private void RegisterAuthentication(IServiceCollection services)
    {
        var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            // ✅ Validate Session GUID after token validation
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

                    var sessionId = context.Principal.FindFirst("SessionGuid")?.Value;
                    var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var userType = context.Principal.FindFirst(ClaimTypes.Role)?.Value;

                    if (string.IsNullOrEmpty(sessionId))
                    {
                        context.Fail("Invalid Session.");
                        return;
                    }

                    var session = await db.UserSessions
                        .FirstOrDefaultAsync(s => s.SessionGuid == sessionId && s.UserId == int.Parse(userId) && s.IsActive);

                    if (session == null)
                    {
                        context.Fail("Session expired or logged in from another device.");
                    }
                }
            };
        });

        services.AddAuthorization();
    }



    // Main pipeline configuration method; this also calls several helper methods.
    public void Configure(IApplicationBuilder app, IHostEnvironment env, ILogger<Startup> logger)
    {
        ConfigureExceptionHandling(app, env, logger);
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        ConfigureSwagger(app, env, logger);
     //   ApplyMigrations(app, logger);

        //this is same and working
        app.UseRouting();
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
          //  endpoints.MapFallbackToFile("index.html");
        });

        LogServerAddresses(app, logger);
        logger.LogInformation("API is ready and running.");
        Console.WriteLine("API is ready and running.");

       
    }

    // Sets up error handling based on the environment.
    private void ConfigureExceptionHandling(IApplicationBuilder app, IHostEnvironment env, ILogger logger)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            logger.LogInformation("Running in production mode.");
            Console.WriteLine("API is running in production mode. Swagger UI is disabled.");
        }
    }

    // Enables Swagger only in development.
    private void ConfigureSwagger(IApplicationBuilder app, IHostEnvironment env, ILogger logger)
    {
        //if (env.IsDevelopment())
        //{
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "swagger";
            });
            logger.LogInformation("Swagger UI is enabled.");
        //}
    }

    // Applies pending EF Core migrations.
    private void ApplyMigrations(IApplicationBuilder app, ILogger logger)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
               // dbContext.Database.Migrate();
                logger.LogInformation("Database migrations applied successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying database migrations.");
                throw;
            }
        }
    }

    // Logs the addresses where the server is listening.
    private void LogServerAddresses(IApplicationBuilder app, ILogger logger)
    {
        var addresses = app.ServerFeatures.Get<IServerAddressesFeature>()?.Addresses;
        if (addresses != null)
        {
            foreach (var address in addresses)
            {
                logger.LogInformation($"Listening on: {address}");
                Console.WriteLine($"Listening on: {address}");
            }
        }
        else
        {
            logger.LogWarning("Unable to log server addresses. IServerAddressesFeature not available.");
        }
    }
}

