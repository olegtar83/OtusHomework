using LegendarySocialNetwork.Domain.Common;
using LegendarySocialNetwork.Domain.Messages;

namespace LegendarySocialNetwork.Application.Events
{
    public class UpdateFeedEventRequested(UpdateFeedMessage eventRequest) : BaseEvent
    {
        public UpdateFeedMessage EventRequest { get; } = eventRequest;
    }
}
