namespace LegendarySocialNetwork.WebApi.DataClasses.Dtos
{
    public class PostDto
    {
        public required string Id { get; set; }
        public required string Text { get; set; }
        public string? Name { get; set; }
        public required string UserId { get; set; }
        public required DateTime Created { get; set; }
    }
}
