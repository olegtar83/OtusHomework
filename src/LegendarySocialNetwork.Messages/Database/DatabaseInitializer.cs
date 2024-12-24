using Dapper;
using Npgsql;
using System.Data;
namespace LegendarySocialNetwork.Messages.Database
{
    public static class DatabaseInitializer
    {
        public static DatabaseSettings DbSetting { get; set; } = new DatabaseSettings();
        public static async Task Init()
        {
            string connectionString = DbSetting.CitusConnStr;

            using (IDbConnection dbConnection = new NpgsqlConnection(connectionString))
            {
                dbConnection.Open();

                string sqlScript = @"CREATE TABLE IF NOT EXISTS messages (
						id int generated always as identity,
                        text TEXT NOT NULL,
                       ""from"" CHARACTER VARYING NOT NULL,
                       ""to"" CHARACTER VARYING NOT NULL,
                       ""shardId"" int NOT NULL,
					   constraint pk primary key(id,""shardId"")
                  );";

                await dbConnection.ExecuteAsync(sqlScript);

                string sqlScript1 = @"CREATE EXTENSION IF NOT EXISTS citus;";

                await dbConnection.ExecuteAsync(sqlScript1);


                try
                {
                    string sqlScript2 = @"SELECT create_distributed_table('messages', 'shardId');";

                    await dbConnection.ExecuteAsync(sqlScript2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
