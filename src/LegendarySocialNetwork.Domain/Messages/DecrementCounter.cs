namespace LegendarySocialNetwork.Domain.Messages
{
    public class DecrementCounter
    {
        public required string From { get; set; }
        public required string To { get; set; }
    }
}
