using Microsoft.EntityFrameworkCore;
using mytown.Models.mytown.DataAccess;

namespace mytown.Services.Implementations
{
    public class ChatCleanupBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ChatCleanupBackgroundService> _logger;
        private static readonly TimeSpan RunInterval = TimeSpan.FromHours(24);
        private const int RetentionDays = 10;

        public ChatCleanupBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<ChatCleanupBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var cutoff = DateTime.UtcNow.AddDays(-RetentionDays);

                    var deletedCount = await db.ChatMessages
                        .Where(m => m.SentTime < cutoff)
                        .ExecuteDeleteAsync(stoppingToken);

                    if (deletedCount > 0)
                    {
                        _logger.LogInformation(
                            "ChatCleanupBackgroundService: deleted {Count} chat messages older than {Days} days.",
                            deletedCount, RetentionDays);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ChatCleanupBackgroundService: error during cleanup run.");
                }

                await Task.Delay(RunInterval, stoppingToken);
            }
        }
    }
}