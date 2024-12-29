using Dapper;
using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;
using LegendarySocialNetwork.Infrastructure.Common.Options;
using MediatR;
using Microsoft.Extensions.Options;
using Npgsql;

namespace LegendarySocialNetwork.Infrastructure.Repositories
{
    public class PostRepository : BaseDbContext, IPostRepository
    {
        public PostRepository(IOptions<DatabaseOptions> settings) : base(settings) { }

        public async Task<Result<string>> CreateAsync(string text, string userId)
        {
            await using var con = await _writeDb.OpenConnectionAsync();

            await using var cmd = new NpgsqlCommand("INSERT INTO public.\"post\"\r\n(id, user_id, text, \"created\", \"updated\")" +
                "\r\nVALUES(@id, @user_id, @text, @created, @updated)\r\n" +
                "RETURNING id;\r\n", con)
            {
                Parameters = {
                new("id", Guid.NewGuid().ToString()),
                new("user_id", userId),
                new("text", text),
                new("created", DateTime.UtcNow),
                new("updated", DateTime.UtcNow),
                }
            };
            var generatedId = await cmd.ExecuteScalarAsync() as string;
            return Result<string>.Success(generatedId!);
        }

        public async Task<Result<Unit>> UpdateAsync(string text, string postId, string userId)
        {
            await using var con = await _writeDb.OpenConnectionAsync();

            await using var cmd = new NpgsqlCommand("UPDATE public.\"post\"\r\n" +
                "SET text = @text, user_id = @user_id, updated = @updated\r\n" +
               "\r\nWHERE id = @postId", con)
            {
                Parameters =
                {new("postId", postId),
                new("user_id", userId),
                new("text", text),
                new("updated", DateTime.UtcNow)}
            };
            await cmd.ExecuteNonQueryAsync();
            return Result<Unit>.Success(Unit.Value);
        }

        public async Task<Result<Unit>> DeleteAsync(string postId)
        {
            await using var con = await _writeDb.OpenConnectionAsync();

            await using var cmd = new NpgsqlCommand("DELETE FROM public.\"post\"\r\n" +
                "\r\nWHERE id = @postId;", con)
            {
                Parameters = {
                new("postId", postId),
                }
            };
            await cmd.ExecuteNonQueryAsync();
            return Result<Unit>.Success(Unit.Value);
        }

        public async Task<Result<PostEntity>> GetAsync(string postId)
        {
            await using var con = await _readDb.OpenConnectionAsync();
            var sql = @"SELECT (u.first_name || ' ' || u.second_name) AS name, p.id, p.user_id, p.text, p.created, p.updated FROM public.post as p
                      inner join public.""user"" as u on p.user_id = u.id
                      WHERE p.id = @postId LIMIT 1;";
            var item = await con.QueryFirstAsync<PostEntity>(sql, new { postId });
            if (item is not null) { return Result<PostEntity>.Success(item); }
            return Result<PostEntity>.Failure("Not found");
        }

        public async Task<Result<List<PostEntity>>> GetLimitedFeedAsync(string user_id, int? limit = 1000)
        {
            await using var con = await _readDb.OpenConnectionAsync();
            var sql = @"SELECT(u.first_name || ' ' || u.second_name) AS name, 
                p.id, p.user_id, p.text, p.created, p.updated FROM public.post as p       
                inner join public.""user"" as u on p.user_id = u.id
                WHERE p.user_id in (SELECT addressed_id FROM public.friendship
                where requester_id = @user_id)
				or p.user_id in (SELECT requester_id FROM public.friendship
                where addressed_id = @user_id)
                LIMIT @limit";
            var items = await con.QueryAsync<PostEntity>(sql, new { user_id, limit });
            return Result<List<PostEntity>>.Success(items.ToList());
        }
    }
}