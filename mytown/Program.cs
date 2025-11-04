using MySqlConnector;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


try
{

    // Prevent ASP.NET from starting when running EF CLI commands
    if (args.Contains("--ef"))
    {
        return;
    }
    // Configure Serilog for both console and file logging.
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console() // Requires Serilog.Sinks.Console package
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Requires Serilog.Sinks.File package
        .CreateLogger();

    // Create the WebApplication builder.
    var builder = WebApplication.CreateBuilder(args);

    // Replace the default logging provider with Serilog.
    Directory.CreateDirectory("logs");
    builder.Host.UseSerilog(); // Requires using Serilog.Extensions.Hosting

    // Load configuration files.
    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);


    // Initialize Startup and register all services.
    var startup = new Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    // Build the application.
    var app = builder.Build();

    // Obtain a logger instance (using Microsoft.Extensions.Logging).
    Microsoft.Extensions.Logging.ILogger logger = app.Services.GetRequiredService<ILogger<Program>>();

    // Test the MySQL connection before starting the app.
   // TestMySQLConnection(builder.Configuration, logger);

    // Configure the HTTP request pipeline via Startup.Configure.
    startup.Configure(app, builder.Environment, app.Services.GetRequiredService<ILogger<Startup>>());

    // Run the application.
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
/// Tests the MySQL connection using the connection string from configuration.
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
