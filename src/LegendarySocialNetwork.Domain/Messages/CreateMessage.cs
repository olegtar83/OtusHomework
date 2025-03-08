namespace LegendarySocialNetwork.Domain.Messages
{
    public class CreateMessage
    {
        public required string From { get; set; }
        public required string To { get; set; }
        public required string Text { get; set; }
    }
}
