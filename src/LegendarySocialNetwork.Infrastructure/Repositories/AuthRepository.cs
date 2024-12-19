using Dapper;
using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;
using LegendarySocialNetwork.Infrastructure.Common.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace LegendarySocialNetwork.Infrastructure.Repositories
{
    public class AuthRepository : BaseDbContext, IAuthRepository
    {
        public AuthRepository(IOptions<DatabaseOptions> settings) : base(settings) { }

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

        public async Task<Result<AccountEntity>> GetLoginAsync(string id)
        {
            await using var con = await _readDb.OpenConnectionAsync();
            var sql = "SELECT id, \"password\" FROM public.account WHERE id = @id LIMIT 1;";
            var item = await con.QueryFirstOrDefaultAsync<AccountEntity>(sql, new { id });
            if (item is not null) { return Result<AccountEntity>.Success(item); }
            return Result<AccountEntity>.Failure("Not found");
        }

    }

}
