
using Microsoft.AspNetCore.Hosting.Server;

namespace LegendarySocialNetwork.Messages.Tarantool.HostedService
{
    public class TarantoolReplicatorService : IHostedLifecycleService
    {
        private readonly IServiceProvider _services;
        private Timer? _timer = null;
        private readonly ILogger<TarantoolReplicatorService> _logger;
        public TarantoolReplicatorService(IServiceProvider services, ILogger<TarantoolReplicatorService> logger)
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
                var tarantoolService = scope.ServiceProvider.GetRequiredService<ITarantoolService>();

                var res = await tarantoolService.Sync();

                _logger.LogInformation($"Tarantool replicated rows: {res}");
            }
            catch(Exception ex)
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
