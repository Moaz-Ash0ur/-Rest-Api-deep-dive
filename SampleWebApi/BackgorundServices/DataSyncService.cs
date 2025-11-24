
namespace SampleWebApi.BackgorundServices
{
    public class DataSyncService : BackgroundService
    {
        private readonly ILogger<DataSyncService> _logger;
        private readonly PeriodicTimer _timer;
        private int count = 0;
        public DataSyncService(ILogger<DataSyncService> logger)
        {
            _logger = logger;
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(10));//just for demo
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Data Sync Service started.");

            try
            {
                while (await _timer.WaitForNextTickAsync(stoppingToken))
                {
                    await SyncData();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Data Sync Service stopped.");
            }
        }

        private Task SyncData()
        {
            
            _logger.LogInformation($"Syncing data at: {DateTime.Now} | Counter : {++count} ");
            //any logic here
            return Task.CompletedTask;
        }



    }


}
