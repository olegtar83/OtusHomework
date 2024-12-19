namespace LegendarySocialNetwork.Domain.Messages
{
    public class PostMessage
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public required string Text { get; set; }
        public DateTime Created { get; set; }
    }
}
