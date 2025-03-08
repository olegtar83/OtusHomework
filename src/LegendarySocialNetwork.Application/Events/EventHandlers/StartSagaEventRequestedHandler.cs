using LegendarySocialNetwork.Domain.Messages;
using MassTransit;
using MediatR;

namespace LegendarySocialNetwork.Application.Events.EventHandlers
{
    public class StartSagaEventRequestedHandler : INotificationHandler<InitSagaEventRequested>
    {
        private readonly ITopicProducer<string, InitSaga> _producer;

        public StartSagaEventRequestedHandler(ITopicProducer<string, InitSaga> producer)
        {
            _producer = producer;
        }
        public async Task Handle(InitSagaEventRequested notification, CancellationToken cancellationToken)
        {
            var command = new InitSaga
            {
                From = notification.EventRequest.From,
                To = notification.EventRequest.To,
                Text = notification.EventRequest.From,
            };

            await _producer.Produce(Guid.NewGuid().ToString(),
                command,
                Pipe.Execute<KafkaSendContext>(p =>
                {
                    p.CorrelationId = Guid.NewGuid();
                }),
                cancellationToken);
        }
    }
}
