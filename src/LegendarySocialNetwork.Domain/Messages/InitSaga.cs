namespace LegendarySocialNetwork.Domain.Messages
{
    public class InitSaga
    {
        public required string From { get; set; }
        public required string To { get; set; }
        public required string Text { get; set; }
    }
}
