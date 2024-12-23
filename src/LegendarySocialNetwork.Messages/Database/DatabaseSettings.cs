namespace LegendarySocialNetwork.Messages.Database
{
    public class DatabaseSettings
    {
        public string CitusConnStr { get; set; } = string.Empty;
        public int ShardsNode { get; set; } = 16;
        public int NodesCount { get; set; } = 2;
    }
}
