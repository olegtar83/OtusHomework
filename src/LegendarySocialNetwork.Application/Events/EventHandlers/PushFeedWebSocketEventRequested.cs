using LegendarySocialNetwork.Domain.Messages;
using MassTransit;
using MediatR;

namespace LegendarySocialNetwork.Application.Events.EventHandlers
{
    public class PushFeedWebSocketEventRequested : INotificationHandler<UpdateFeedEventRequested>
    {
        private readonly ITopicProducerProvider _topicProducerProvider;

        public PushFeedWebSocketEventRequested(ITopicProducerProvider topicProducerProvider)
        {
            _topicProducerProvider = topicProducerProvider;
        }
        public async Task Handle(UpdateFeedEventRequested notification, CancellationToken cancellationToken)
        {
            if (notification.EventRequest.Operation == Operation.Create)
            {
                var producer = _topicProducerProvider.GetProducer<UpdateFeedMessage>(
                    new Uri($"topic:{Environment.GetEnvironmentVariable("Kafka:PushFeedTopic")}"));

                await producer.Produce(notification.EventRequest, cancellationToken);
            }
            else
                await Task.CompletedTask;
        }
    }
}
