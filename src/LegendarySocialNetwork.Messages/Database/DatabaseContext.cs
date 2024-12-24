using Dapper;
using LegendarySocialNetwork.Messages.Database.Entities;
using LegendarySocialNetwork.Messages.DataClasses.Models;
using LegendarySocialNetwork.Messages.Utilities;
using Microsoft.Extensions.Options;
using Npgsql;

namespace LegendarySocialNetwork.Messages.Database;

public class DatabaseContext : IDatabaseContext, IDisposable
{
    public DatabaseContext(IOptions<DatabaseSettings> settings)
    {
        connStr = settings.Value.CitusConnStr;
        db = NpgsqlDataSource.Create(connStr);
    }
    private readonly string connStr;
    private readonly NpgsqlDataSource db;
    public async void Dispose()
    {
        await db.DisposeAsync();
    }

    public async Task<Result<string>> SetDialogAsync(string from, string to, string text)
    {
        await using var con = await db.OpenConnectionAsync();

        await using var cmd = new NpgsqlCommand("INSERT INTO public.messages\r\n" +
            "(text, \"from\", \"to\", \"shardId\")\r\n" +
            "VALUES(@text, @from, @to, @shardId)\r\n" +
            "RETURNING id;", con)
        {
            Parameters =    {
                new("from", from),
                new("to", to),
                new("text", text),
                new("shardId", HashUtility.GetHashId(from, to)),
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
}