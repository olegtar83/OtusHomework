namespace LegendarySocialNetwork.Domain.Messages
{
    public class IncrementCounter
    {
        public required string From { get; set; }
        public required string To { get; set; }
    }
}
