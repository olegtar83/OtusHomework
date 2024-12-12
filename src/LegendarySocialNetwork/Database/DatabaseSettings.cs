namespace LegendarySocialNetwork.Database;
public class DatabaseSettings
{
    public string MasterConnStr { get; set; } = string.Empty;
    public List<string> ReplicaConnStrings { get; set; } = new();

    public DatabaseSettings()
    {
        ReplicaConnStrings.Add(Environment.GetEnvironmentVariable("DatabaseSettings:ReplicaConnStr1")!);
        ReplicaConnStrings.Add(Environment.GetEnvironmentVariable("DatabaseSettings:ReplicaConnStr2")!);
    }
}