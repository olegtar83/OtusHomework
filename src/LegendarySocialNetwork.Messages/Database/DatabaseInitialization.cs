using Dapper;
using LegendarySocialNetwork.Messages.Services;
using Npgsql;
using System.Data;

namespace LegendarySocialNetwork.Messages.Database
{
    public static class DatabaseInitialization
    {
        public static DatabaseSettings DbSetting { get; set; } = new DatabaseSettings();
        public static void Init()
        {
            // Define your connection string
            string connectionString = DbSetting.CitusConnStr;

            using (IDbConnection dbConnection = new NpgsqlConnection(connectionString))
            {
                dbConnection.Open();

                // Load your SQL script from a file or define it as a string
                string sqlScript = @"CREATE TABLE IF NOT EXISTS messages (
                            id CHARACTER VARYING NOT NULL,
                            text TEXT NOT NULL,
                            ""from"" CHARACTER VARYING NOT NULL,
                            ""to"" CHARACTER VARYING NOT NULL,
	                        ""shardId"" int NOT NULL
                        );";

                // Execute the SQL script
                dbConnection.Execute(sqlScript);

                string sqlScript1 = @"CREATE EXTENSION IF NOT EXISTS citus;";

                dbConnection.Execute(sqlScript1);

     
                try
                {
                    string sqlScript2 = @"SELECT create_distributed_table('messages', 'shardId');";

                    dbConnection.Execute(sqlScript2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
