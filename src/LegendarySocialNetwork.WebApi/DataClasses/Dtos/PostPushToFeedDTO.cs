namespace LegendarySocialNetwork.WebApi.DataClasses.Dtos
{
    public class PostPushToFeedDTO
    {
        public required string TargetedUserId { get; set; }
        public required PostDto Post { get; set; }
    }
}
