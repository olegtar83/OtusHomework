namespace LegendarySocialNetwork.Messages.Database.Entities
{
    public class DialogEntity
    {
        public required string From { get; set; }
        public required string To { get; set; }
        public required string Text { get; set; }
        public required string ShardId { get; set; }
    }
}
