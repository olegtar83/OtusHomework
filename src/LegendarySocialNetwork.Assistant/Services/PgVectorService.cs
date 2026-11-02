using Dapper;
using Microsoft.Extensions.AI;
using Npgsql;
using Pgvector;

namespace LegendarySocialNetwork.Assistant.Service;

public interface IPgVectorService
{
    Task<int> Sync();
    Task<IReadOnlyList<DocumentMatch>> SearchAsync(string query, int topK = 5);
}

public class PgVectorService(
[FromKeyedServices("messages-db")] NpgsqlDataSource dataSourceMessages, 
[FromKeyedServices("pg-vector")] NpgsqlDataSource dataSourceVectors,
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
ILogger<PgVectorService> logger) : IPgVectorService
{
    private readonly NpgsqlDataSource _dataSourceMessages = dataSourceMessages;
    private readonly NpgsqlDataSource _dataSourceVectors = dataSourceVectors;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator = embeddingGenerator;
    private readonly ILogger<PgVectorService> _logger = logger;
    private const int EmbedBatch = 256;

    private static async Task InsertBatchAsync(NpgsqlConnection conn, DocumentInsert[] rows)
    {
        var values = string.Join(
            ",",
            Enumerable.Range(0, rows.Length).Select(i => $"('message', @s{i}, @u{i}, @t{i}, @e{i})"));
        var args = new DynamicParameters();
        for (var i = 0; i < rows.Length; i++)
        {
            args.Add($"s{i}", rows[i].SourceId);
            args.Add($"u{i}", rows[i].UserId);
            args.Add($"t{i}", rows[i].Text);
            args.Add($"e{i}", rows[i].Embedding);
        }

        await conn.ExecuteAsync($"""
            INSERT INTO documents (source, source_id, user_id, text, embedding)
            VALUES {values}
            ON CONFLICT (source, source_id) DO UPDATE
            SET user_id = EXCLUDED.user_id,
                text = EXCLUDED.text,
                embedding = EXCLUDED.embedding
            """, args);
    }

    const string sql = """
        SELECT id,
               "from" AS from_user,
               "to" AS to_user,
               text
        FROM public.messages
        WHERE id > @lastId
        ORDER BY id
        """;

    public async Task<int> Sync()
    {
        await using var vectorConn = await _dataSourceVectors.OpenConnectionAsync();
        var lastId = await vectorConn.ExecuteScalarAsync<int>("""
            SELECT COALESCE(MAX(source_id::int), 0)
            FROM documents
            WHERE source = 'message'
            """);

        await using var conn = await _dataSourceMessages.OpenConnectionAsync();
        var count = 0;

        await foreach (var chunk in StreamMessagesInChunksAsync(conn, lastId))
        {
            var embeddings = await _embeddingGenerator.GenerateAsync(
                chunk.Select(row => "passage: " + row.text).ToArray());
            var rows = chunk.Select((row, i) => new DocumentInsert(
                row.id.ToString(),
                row.from_user,
                row.text,
                new Vector(embeddings[i].Vector.ToArray()))).ToArray();

            await InsertBatchAsync(vectorConn, rows);
            count += chunk.Count;
            _logger.LogInformation(
                "Stored {Count} messages, ids {FirstId}-{LastId}, total {Total}",
                chunk.Count, chunk[0].id, chunk[^1].id, count);
        }

        return count;
    }

    public async Task<IReadOnlyList<DocumentMatch>> SearchAsync(string query, int topK = 5)
    {
        var embedding = await _embeddingGenerator.GenerateAsync("query: " + query);
        var vector = new Vector(embedding.Vector.ToArray());

        await using var conn = await _dataSourceVectors.OpenConnectionAsync();
        var results = await conn.QueryAsync<DocumentMatch>("""
            SELECT user_id AS UserId,
                   text AS Text,
                   distance AS Distance
            FROM (
                SELECT DISTINCT ON (text)
                       user_id,
                       text,
                       embedding <=> @vector AS distance
                FROM documents
                WHERE source = 'message'
                ORDER BY text, embedding <=> @vector
            ) unique_texts
            ORDER BY Distance
            LIMIT @topK
            """, new { vector, topK });

        return results.AsList();
    }

    private async IAsyncEnumerable<IReadOnlyList<MessageRow>> StreamMessagesInChunksAsync(
        NpgsqlConnection conn,
        int startId,
        int chunkSize = EmbedBatch,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var chunk in conn
            .QueryUnbufferedAsync<MessageRow>(sql, new { lastId = startId })
            .Chunk(chunkSize)
            .WithCancellation(cancellationToken))
        {
            yield return chunk;
        }
    }
}

public sealed record DocumentInsert(string SourceId, string UserId, string Text, Vector Embedding);

public sealed record DocumentMatch(string UserId, string Text, double Distance);
public sealed record MessageRow(int id, string from_user, string to_user, string text);
