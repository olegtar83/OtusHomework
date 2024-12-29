using Dapper;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Database;
using LegendarySocialNetwork.Domain.Entities;
using LegendarySocialNetwork.Infrastructure.Common.Options;
using Microsoft.Extensions.Options;

namespace LegendarySocialNetwork.Infrastructure.Repositories
{
    public class UserRepository : BaseDbContext, IUserRepository
    {
        public UserRepository(IOptions<DatabaseOptions> settings) : base(settings) { }

        public async Task<Result<UserEntity>> GetUserAsync(string id)
        {
            await using var con = await _readDb.OpenConnectionAsync();

            var sql = "SELECT id, first_name, second_name, sex, age, city, biography\r\nFROM public.\"user\"\r\n WHERE id = @id LIMIT 1;";
            var item = await con.QueryFirstOrDefaultAsync<UserEntity>(sql, new { id });
            if (item is not null) { return Result<UserEntity>.Success(item); }
            return Result<UserEntity>.Failure("Not found");
        }

        public async Task<Result<List<UserEntity>>> SearchUserAsync(string? firstName, string? lastName)
        {
            await using var con = await _readDb.OpenConnectionAsync();
            var sql = "SELECT id, first_name, second_name, sex, age, city, biography\r\nFROM public.\"user\"\r\n";
            var sqlConditions = new List<string>();
            IEnumerable<UserEntity> items;
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                sql += "WHERE first_name ILIKE @firstname AND second_name ILIKE @secondname ORDER BY id;";
                items = con.Query<UserEntity>(sql, new
                {
                    @firstname = $"{firstName}%",
                    @secondname = $"{lastName}%",
                });
            }
            else if (!string.IsNullOrEmpty(firstName))
            {
                sql += "WHERE first_name ILIKE @firstname ORDER BY id;";
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

                sql += "WHERE first_name ILIKE @firstname AND second_name ILIKE @secondname ORDER BY id;";
                items = await con.QueryAsync<UserEntity>(sql, new
                {
                    firstname = $"{firstName}%",
                    secondname = $"{lastName}%",
                });
            }

            return Result<List<UserEntity>>.Success(items.ToList());
        }
    }
}
