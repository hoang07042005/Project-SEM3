using eAdministrationLabs.Models;

namespace eAdministrationLabs.Services
{
    public class LabStatusUpdater : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public LabStatusUpdater(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<EAdministrationLabsContext>();

                    var labsToUpdate = context.LabUsageLogs
                        .Where(l => l.EndTime <= DateTime.Now)
                        .ToList();

                    foreach (var labUsageLog in labsToUpdate)
                    {
                        var lab = context.Labs.FirstOrDefault(l => l.Id == labUsageLog.LabId);
                        if (lab != null)
                        {
                            var inactiveStatus = context.StatusLabs
                                .FirstOrDefault(s => s.StatusName == "Inactive");
                            if (inactiveStatus != null)
                            {
                                lab.StatusLabId = inactiveStatus.Id;
                            }
                        }
                    }

                    await context.SaveChangesAsync();
                }

                // Check every minute
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}
