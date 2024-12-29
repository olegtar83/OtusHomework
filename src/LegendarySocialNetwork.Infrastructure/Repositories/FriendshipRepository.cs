using Dapper;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;
using LegendarySocialNetwork.Infrastructure.Common.Options;
using MediatR;
using Microsoft.Extensions.Options;
using Npgsql;

namespace LegendarySocialNetwork.Infrastructure.Repositories
{
    public class FriendshipRepository : BaseDbContext, IFriendshipRepository
    {
        public FriendshipRepository(IOptions<DatabaseOptions> settings) : base(settings) { }

        public async Task<Result<Unit>> SetAsync(string requesterUserId, string addressedUserId)
        {
            await using var con = await _writeDb.OpenConnectionAsync();

            await using var cmd = new NpgsqlCommand("INSERT INTO public.\"friendship\"\r\n(requester_id, addressed_id, \"created\", \"updated\")" +
                "\r\nVALUES(@requester_id, @addressed_id, @created, @updated);\r\n", con)
            {
                Parameters = {
                new("requester_id", requesterUserId),
                new("addressed_id", addressedUserId),
                new("created", DateTime.UtcNow),
                new("updated", DateTime.UtcNow),
                }
            };
            await cmd.ExecuteNonQueryAsync();
            return Result<Unit>.Success(Unit.Value);
        }

        public async Task<Result<Unit>> DeleteAsync(string requesterUserId, string addressedUserId)
        {
            await using var con = await _writeDb.OpenConnectionAsync();

            await using var cmd = new NpgsqlCommand("DELETE FROM public.\"friendship\"\r\n" +
                "\r\nWHERE requester_id = @requester_id and addressed_id = @addressed_id" +
                " or addressed_id = @requester_id and requester_id = @addressed_id", con)
            {
                Parameters = {
                new("requester_id", requesterUserId),
                new("addressed_id", addressedUserId),
                }
            };
            await cmd.ExecuteNonQueryAsync();
            return Result<Unit>.Success(Unit.Value);
        }

        public async Task<Result<IEnumerable<FriendshipEntity>>> GetAsync(string userId)
        {
            await using var con = await _readDb.OpenConnectionAsync();
            var sql = "SELECT requester_id, addressed_id, created, updated FROM public.friendship \r\n" +
                "WHERE requester_id = @userId or addressed_id = @userId;";
            var items = await con.QueryAsync<FriendshipEntity>(sql, new { userId });
            return Result<IEnumerable<FriendshipEntity>>.Success(items);
        }

        public async Task<Result<List<FriendEntity>>> GetFriendsAsync(string user_id)
        {
            await using var con = await _readDb.OpenConnectionAsync();
            var sql = @"SELECT DISTINCT
                u.id, 
                (u.first_name || ' ' || u.second_name) AS name, 
                u.city
            FROM 
                public.user AS u
            WHERE 
                u.id IN (SELECT addressed_id FROM public.friendship WHERE requester_id = @user_id)
                OR u.id IN (SELECT requester_id FROM public.friendship WHERE addressed_id = @user_id);";
            var items = await con.QueryAsync<FriendEntity>(sql, new { user_id });
            return Result<List<FriendEntity>>.Success(items.ToList());
        }
    }
}
