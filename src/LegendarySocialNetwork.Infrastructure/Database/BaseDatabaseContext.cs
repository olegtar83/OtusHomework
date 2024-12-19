using LegendarySocialNetwork.Infrastructure.Common.Options;
using Microsoft.Extensions.Options;
using Npgsql;
public abstract class BaseDbContext : IDisposable
{
    protected readonly NpgsqlDataSource _readDb;
    protected readonly NpgsqlDataSource _writeDb;

    public BaseDbContext(IOptions<DatabaseOptions> settings)
    {
        if (settings.Value.ReplicaConnStrings.Any())
        {
            var readConnStr = settings.Value.ReplicaConnStrings[new Random().Next(settings.Value.ReplicaConnStrings.Count)];
            _readDb = NpgsqlDataSource.Create(readConnStr);
            _writeDb = NpgsqlDataSource.Create(settings.Value.MasterConnStr);
        }
        else
        {
            _readDb = NpgsqlDataSource.Create(settings.Value.MasterConnStr);
            _writeDb = NpgsqlDataSource.Create(settings.Value.MasterConnStr);
        }
    }

    public async void Dispose()
    {
        await _readDb.DisposeAsync();
        await _writeDb.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
