namespace LegendarySocialNetwork.Infrastructure.Common.Options;
public class DatabaseOptions
{
    public string MasterConnStr { get; set; } = string.Empty;
    public List<string> ReplicaConnStrings { get; set; } = new();

    public DatabaseOptions()
    {
        if (Environment.GetEnvironmentVariable("DatabaseSettings:ReplicaConnStr1") != null)
            ReplicaConnStrings.Add(Environment.GetEnvironmentVariable("DatabaseSettings:ReplicaConnStr1")!);

        if (Environment.GetEnvironmentVariable("DatabaseSettings:ReplicaConnStr2") != null)
            ReplicaConnStrings.Add(Environment.GetEnvironmentVariable("DatabaseSettings:ReplicaConnStr2")!);
    }
}