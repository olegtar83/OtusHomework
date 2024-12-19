using LegendarySocialNetwork.Domain.Entities.Base;

namespace LegendarySocialNetwork.Domain.Entities
{
    public class FriendshipEntity : BaseEntity
    {
        public required string Requester_id { get; set; }
        public required string Addressed_id { get; set; }
    }
}
