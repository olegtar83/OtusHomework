using Dapper;
using Npgsql;

public static class DatabaseInitializer
{
        private static NpgsqlDataSource? _dataSource;

        public static async Task Init(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;

                    
            await using var conn = await dataSource.OpenConnectionAsync();

            await using var enableExt = new NpgsqlCommand(
                "CREATE EXTENSION IF NOT EXISTS vector", conn);
            await enableExt.ExecuteNonQueryAsync();

            conn.ReloadTypes();

            await conn.ExecuteAsync(
                """
                CREATE TABLE IF NOT EXISTS documents (
                    id SERIAL PRIMARY KEY,
                    source TEXT NOT NULL,
                    source_id TEXT NOT NULL,
                    user_id TEXT NOT NULL,
                    text TEXT NOT NULL,
                    embedding vector(384) NOT NULL,
                    UNIQUE (source, source_id)
                )
                """);

            await conn.ExecuteAsync(
                """
                CREATE INDEX IF NOT EXISTS documents_embedding_idx
                ON documents USING hnsw (embedding vector_cosine_ops)
                """);
        }

}