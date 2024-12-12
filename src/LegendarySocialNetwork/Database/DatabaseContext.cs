using Dapper;
using LegendarySocialNetwork.Database.Entities;
using LegendarySocialNetwork.DataClasses.Internals;
using Microsoft.Extensions.Options;
using Npgsql;

namespace LegendarySocialNetwork.Database;

public class DatabaseContext : IDatabaseContext, IDisposable
{
    public DatabaseContext(IOptions<DatabaseSettings> settings)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        NpgsqlLoggingConfiguration.InitializeLogging(loggerFactory);
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
    private readonly NpgsqlDataSource _readDb;
    private readonly NpgsqlDataSource _writeDb;
    public async void Dispose()
    {
        await _readDb.DisposeAsync();
        await _writeDb.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    public async Task<Result<AccountEntity>> GetLoginAsync(string id)
    {
        await using var con = await _readDb.OpenConnectionAsync();
        var sql = "SELECT id, \"password\" FROM public.account WHERE id = @id LIMIT 1;";
        var item = await con.QueryFirstOrDefaultAsync<AccountEntity>(sql, new { id });
        if (item is not null) { return Result<AccountEntity>.Success(item); }
        return Result<AccountEntity>.Failure("Not found");
    }

    public async Task<Result<string>> RegisterAsync(UserEntity user, string password)
    {
        await using var con = await _writeDb.OpenConnectionAsync();
   
        await using var cmdAccount = new NpgsqlCommand("INSERT INTO public.account\r\n(id, \"password\")\r\nVALUES(@id, @password);\r\n", con)
        {
            Parameters = {
                new("id", user.Id),
                new("password", password)
            }
        };
        await cmdAccount.ExecuteNonQueryAsync();

        // Create user
        await using var cmdUser = new NpgsqlCommand("INSERT INTO public.\"user\"\r\n(id, first_name, second_name, sex, age, city, biography)\r\n" +
            "VALUES(@id, @firstname, @secondname, @sex, @age, @city, @biography);\r\n", con)
        {
            Parameters = {
                new("id", user.Id),
                new("firstname", user.First_name),
                new("secondname", user.Second_name),
                new("sex", user.Sex),
                new("age", user.Age),
                new("city", user.City),
                new("biography", user.Biography)
            }
        };
        await cmdUser.ExecuteNonQueryAsync();

        return Result<string>.Success(user.Id);
    }

    public async Task<Result<UserEntity>> GetUserAsync(string id)
    {
        await using var con = await _readDb.OpenConnectionAsync();

        var sql = "SELECT id, first_name, second_name, sex, age, city, biography\r\nFROM public.\"user\"\r\n WHERE id = @id LIMIT 1;";
        var item = await con.QueryFirstOrDefaultAsync<UserEntity>(sql, new { id });
        if (item is not null) { return Result<UserEntity>.Success(item); }
        return Result<UserEntity>.Failure("Not found");
    }

    public async Task<Result<List<UserEntity>>> SearchUserAsync(string firstName, string lastName)
    {
        await using var con = await _readDb.OpenConnectionAsync();
        var sql = "SELECT id, first_name, second_name, sex, age, city, biography\r\nFROM public.\"user\"\r\n";
        var sqlConditions = new List<string>();
        IEnumerable<UserEntity> items;
        if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
        {
            sql += "WHERE first_name LIKE @firstname AND second_name LIKE @secondname ORDER BY id;";
            items = con.Query<UserEntity>(sql, new
            {
                @firstname = $"{firstName}%",
                @secondname = $"{lastName}%",
            });
        }
        else if (!string.IsNullOrEmpty(firstName))
        {
            sql += "WHERE first_name LIKE @firstname ORDER BY id;";
            items = await con.QueryAsync<UserEntity>(sql, new
            {
                @firstname = $"{firstName}%"
            });
        }
        else if (!string.IsNullOrEmpty(lastName))
        {
            sql += "WHERE second_name LIKE @secondname ORDER BY id;";
            items = await con.QueryAsync<UserEntity>(sql, new
            {
                @secondname = $"{lastName}%"
            });
        }
        else
        {
            firstName = RandomNames.GetRandomFirstName();
            lastName = RandomNames.GetRandomSecondName();

            sql += "WHERE first_name LIKE @firstname AND second_name LIKE @secondname ORDER BY id;";
            items =await con.QueryAsync<UserEntity>(sql, new
            {
                firstname = $"{firstName}%",
                secondname = $"{lastName}%",
            });
        }

        return Result<List<UserEntity>>.Success(items.ToList());
    }
}
