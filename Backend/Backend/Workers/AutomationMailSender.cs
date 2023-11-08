using Backend.Services.IServices;

namespace Backend.Workers
{
    public class AutomationMailSender : BackgroundService
    {
        private readonly ILogger<IReminderService> _logger;
        private readonly IServiceProvider _services;

        public AutomationMailSender(
            ILogger<IReminderService> logger,
            IServiceProvider services
        )
        {
            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
                        await reminderService.Run();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while sending the email.");
                }

                // Here you could specify how often the email should be sent, for example every day, week, etc.
                await Task.Delay(TimeSpan.FromMinutes(AppSettings.ChroneJobTimeInterval), stoppingToken);
            }
        }
    }
}
