
using Dapper;
using LegendarySocialNetwork.Messages.Database.Entities;
using LegendarySocialNetwork.Messages.DataClasses.Models;
using Microsoft.Extensions.Options;

using Npgsql;

namespace LegendarySocialNetwork.Messages.Database;

public class DatabaseContext : IDatabaseContext, IDisposable
{
    public DatabaseContext(IOptions<DatabaseSettings> settings, ILogger<DatabaseContext> logger)
    {
        connStr = settings.Value.CitusConnStr;
        _logger = logger;
        db = NpgsqlDataSource.Create(connStr);
    }
    private readonly string connStr;
    private readonly NpgsqlDataSource db;
    private readonly ILogger<DatabaseContext> _logger;
    public async void Dispose()
    {
        await db.DisposeAsync();
    }

    public async Task<Result<string>> SetDialogAsync(string id, 
        string from, string to, string text, int shardId)
    {
        await using var con = await db.OpenConnectionAsync();
        // Create account
        await using var cmd = new NpgsqlCommand("INSERT INTO public.messages\r\n" +
            "(id, \"from\", \"to\", \"text\", \"shardId\")\r\n" +
            "VALUES(@id, @from, @to, @text, @shardId)\r\n" +
            "RETURNING id;", con)
        {
            Parameters =    {
                new("id",id),
                new("from", from),
                new("to", to),
                new("text", text),
                new("shardId", shardId)
            }
        };
        var generatedId = await cmd.ExecuteScalarAsync() as string;
        return Result<string>.Success(generatedId!);
    }

    public async Task<Result<List<DialogEntity>>> GetDialogsAsync(string id)
    {
        await using var con = await db.OpenConnectionAsync();
        var sql = "SELECT \"from\", \"to\", \"text\" FROM public.messages WHERE  \"from\" = @id or \"to\" = @id;";
        var item = await con.QueryAsync<DialogEntity>(sql, new { id });
        if (item is not null) { return Result<List<DialogEntity>>.Success(item.ToList()); }
        return Result<List<DialogEntity>>.Failure("Not found");
    }

    public async Task<Result<string>> GetDialogIdAsync(string from, string to)
    {
        await using var con = await db.OpenConnectionAsync();
        var sql = "SELECT id FROM public.\"messages\" WHERE \"from\" = @from or \"to\" = @to LIMIT 1;";
        var item = await con.QueryAsync<string>(sql, new { from, to });
        if (item.Any()) { return Result<string>.Success(item.First()); }
        return Result<string>.Failure("Not found");
    }
}