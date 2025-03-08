using LegendarySocialNetwork.Domain.Common;
using LegendarySocialNetwork.Domain.Messages;

namespace LegendarySocialNetwork.Application.Events
{
    public class InitSagaEventRequested(InitSaga eventRequest) : BaseEvent
    {
        public InitSaga EventRequest { get; } = eventRequest;
    }
}
