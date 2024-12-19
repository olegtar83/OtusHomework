using LegendarySocialNetwork.Domain.Messages;
using MassTransit;
using MediatR;

namespace LegendarySocialNetwork.Application.Events.EventHandlers
{
    internal sealed class UpdateCachedFeedEventHandler : INotificationHandler<UpdateFeedEventRequested>
    {
        private readonly ITopicProducerProvider _topicProducerProvider;

        public UpdateCachedFeedEventHandler(ITopicProducerProvider topicProducerProvider)
        {
            _topicProducerProvider = topicProducerProvider;
        }

        public async Task Handle(UpdateFeedEventRequested notification, CancellationToken cancellationToken)
        {
            var producer = _topicProducerProvider.GetProducer<UpdateFeedMessage>(
                new Uri($"topic:{Environment.GetEnvironmentVariable("Kafka:UpdateFeedTopic")}"));

            await producer.Produce(notification.EventRequest, cancellationToken);
        }
    }
}
