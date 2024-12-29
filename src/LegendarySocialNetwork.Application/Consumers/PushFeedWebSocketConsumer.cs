using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Domain.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LegendarySocialNetwork.Application.Consumers
{
    public class PushFeedWebSocketConsumer : IConsumer<UpdateFeedMessage>
    {
        private readonly ILogger<PushFeedWebSocketConsumer> _logger;
        private readonly IPushHubNotifier _pushHubNotifier;
        public PushFeedWebSocketConsumer(ILogger<PushFeedWebSocketConsumer> logger,
            IPushHubNotifier pushHubNotifier)
        {
            _logger = logger;
            _pushHubNotifier = pushHubNotifier;
        }
        public async Task Consume(ConsumeContext<UpdateFeedMessage> context)
        {
            foreach (var userId in context.Message.FriendsIds)
            {
                _logger.LogInformation($"Pushing post {context.Message.Post!.Text} to user #{userId}");

                await _pushHubNotifier.PublishAsync(context.Message.Post, userId);
            }
        }
    }
}
