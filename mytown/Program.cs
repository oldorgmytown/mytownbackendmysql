using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using mytown.Filters;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Services.Interfaces;
using mytown.Services.Implementations;
using Serilog;
using System.Text;

try
{
    // Prevent ASP.NET from starting when running EF CLI commands
    if (args.Contains("--ef"))
    {
        return;
    }

    // Configure Serilog
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    // Create builder
    var builder = WebApplication.CreateBuilder(args);

    // ✅ FIX 1: Azure container port binding
    builder.WebHost.UseUrls("http://0.0.0.0:80");

    // Logging setup
    Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "logs"));
    builder.Host.UseSerilog();

    // Load config
    builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    // Controllers
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidateModelAttribute>();
    })
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

    // ✅ FIX 2: Dependency Injection (VERY IMPORTANT)
    builder.Services.AddScoped<IBusinessServiceRepository, BusinessServiceRepository>();
    builder.Services.AddScoped<IBusinessServiceService, BusinessServiceService>();

    // Startup class
    var startup = new Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    var app = builder.Build();

    var logger = app.Services.GetRequiredService<ILogger<Program>>();

    // Optional DB test
    // TestMySQLConnection(builder.Configuration, logger);

    startup.Configure(app, builder.Environment, app.Services.GetRequiredService<ILogger<Startup>>());

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Application startup error: {ex}");
    Log.Fatal(ex, "Application startup error");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// MySQL Test
static void TestMySQLConnection(IConfiguration configuration, Microsoft.Extensions.Logging.ILogger logger)
{
    var connStr = configuration.GetConnectionString("mysqlConnection");

    using (var conn = new MySqlConnection(connStr))
    {
        try
        {
            conn.Open();
            logger.LogInformation("MySQL connection successful.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MySQL connection failed.");
        }
    }
}