using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using mytown.Filters;
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

    // Create the WebApplication builder
    var builder = WebApplication.CreateBuilder(args);

    
   builder.WebHost.UseUrls("http://0.0.0.0:80");

    // Setup logging
    Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "logs"));
    builder.Host.UseSerilog();

    // Load configuration
    builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    // Add controllers + validation filter
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidateModelAttribute>();
    })
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

    // Initialize Startup
    var startup = new Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    // Build app
    var app = builder.Build();

    // Logger instance
    Microsoft.Extensions.Logging.ILogger logger =
        app.Services.GetRequiredService<ILogger<Program>>();

    // Configure pipeline
    startup.Configure(app, builder.Environment,
        app.Services.GetRequiredService<ILogger<Startup>>());

    // Run app
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Application startup error: {ex.Message}");
    Log.Fatal(ex, "Application startup error");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>
/// Tests MySQL connection (optional)
/// </summary>
static void TestMySQLConnection(IConfiguration configuration, Microsoft.Extensions.Logging.ILogger logger)
{
    var connStr = configuration.GetConnectionString("mysqlConnection");

    using (var conn = new MySqlConnection(connStr))
    {
        try
        {
            conn.Open();
            logger.LogInformation("MySQL connection successful.");
            Console.WriteLine("Connection successful now!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MySQL connection failed.");
            Console.WriteLine("Connection failed: " + ex.Message);
        }
    }
}