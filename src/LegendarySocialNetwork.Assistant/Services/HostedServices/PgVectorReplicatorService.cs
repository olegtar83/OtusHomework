
using LegendarySocialNetwork.Assistant.Service;

namespace LegendarySocialNetwork.Assistant.HostedService
{
    public class PgVectorReplicatorService : IHostedLifecycleService
    {
        private readonly IServiceProvider _services;
        private Timer? _timer = null;
        private readonly ILogger<PgVectorReplicatorService> _logger;
        public PgVectorReplicatorService(IServiceProvider services, ILogger<PgVectorReplicatorService> logger)
        {
            _services = services;
            _logger = logger;   
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task StartedAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(60 * 1000, cancellationToken);
                _timer = new Timer(async o => await DoWork(o, cancellationToken), null, TimeSpan.Zero,
                        TimeSpan.FromSeconds(1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex?.ToString());
            }

        }

        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async Task StoppedAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            await Task.CompletedTask;
        }

        public async Task StoppingAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        private readonly SemaphoreSlim _throttler = new(1, 1);
        private async Task DoWork(object? state, CancellationToken cancellationToken)
        {
            await _throttler.WaitAsync(cancellationToken);

            try
            {
                _ = state;

                using var scope = _services.CreateScope();
                var pgVectorService = scope.ServiceProvider.GetRequiredService<IPgVectorService>();

                var res = await pgVectorService.Sync();    
                var period = res == 0 ? TimeSpan.FromSeconds(30) : TimeSpan.FromSeconds(1);
                _timer?.Change(period, period);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), ex?.ToString());
            }
            finally
            {
                _throttler.Release();
            }
        }
    }
}
