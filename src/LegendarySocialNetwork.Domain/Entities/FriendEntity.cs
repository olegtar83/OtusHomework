using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegendarySocialNetwork.Domain.Entities
{
    public class FriendEntity
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string City { get; set; }
    }
}
