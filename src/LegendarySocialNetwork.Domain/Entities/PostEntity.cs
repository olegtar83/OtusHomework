using LegendarySocialNetwork.Domain.Entities.Base;

namespace LegendarySocialNetwork.Domain.Entities
{
    public class PostEntity : BaseEntity
    {
        public required string Id { get; set; }
        public required string User_id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? Name { get; set; }
    }
}
