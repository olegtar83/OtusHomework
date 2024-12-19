namespace LegendarySocialNetwork.Domain.Messages
{
    public class UpdateFeedMessage
    {
        public PostMessage? Post { get; set; }
        public IEnumerable<string> FriendsIds { get; set; } = [];
        public Operation Operation { get; set; }
        public string? UserId { get; set; }
    }

    public enum Operation
    {
        Create,
        Update,
        Delete,
        Reset
    }
}
