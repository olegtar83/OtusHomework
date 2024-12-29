using AutoMapper;
using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Domain.Messages;
using LegendarySocialNetwork.WebApi.DataClasses.Dtos;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System.Collections.Concurrent;
using System.Text.Json;

namespace LegendarySocialNetwork.WebApi.Hubs
{
    public class PushHubNotifier : IPushHubNotifier
    {
        private readonly ILogger<PushHubNotifier> _logger;
        private readonly IHubContext<FeedHub, IFeedClient> _hubContext;
        private readonly IRedisCacheClient _redisCacheClient;
        private readonly IMapper _mapper;
        private readonly ConcurrentBag<Guid> _sentMessages = new ConcurrentBag<Guid>();

        public PushHubNotifier(ILogger<PushHubNotifier> logger,
            IHubContext<FeedHub, IFeedClient> hubContext,
            IRedisCacheClient redisCacheClient,
            IMapper mapper)
        {
            _logger = logger;
            _hubContext = hubContext;
            _redisCacheClient = redisCacheClient;
            _mapper = mapper;
        }

        public async Task SubscribeToFeedAsync(string userId)
        {
            _logger.LogInformation($"Feed Subscried for user - {userId}");

            var subscriber = _redisCacheClient.GetDbFromConfiguration().Database.Multiplexer.GetSubscriber();

            await subscriber.SubscribeAsync(RedisChannel.Literal(userId), PushPost);
        }

        public async Task UnsubscribeFromFeedAsync(string userId)
        {
            _logger.LogInformation($"Feed Unsubscried for user - {userId}");

            var subscriber = _redisCacheClient.GetDbFromConfiguration().Database.Multiplexer.GetSubscriber();

            await subscriber.UnsubscribeAsync(RedisChannel.Literal(userId), PushPost);
        }

        private async void PushPost(RedisChannel channel, RedisValue message)
        {
            _logger.LogInformation($"Message arrived from Redis - {message}");

            var pushObject = JsonSerializer.Deserialize<PostPushToFeedDTO>(message.ToString());

            var userId = pushObject!.TargetedUserId;

            if (!_sentMessages.Contains(pushObject.MessageId))
            {
                await _hubContext.Clients.User(userId)
                   .PushToFeed(pushObject.Post);

                _sentMessages.Add(pushObject.MessageId);
            }
        }

        public async Task PublishAsync(PostMessage meesage, string userId)
        {
            _logger.LogInformation($"Pushing post to user -{userId}");

            var messageId = Guid.NewGuid();
            var updateDto = new PostPushToFeedDTO
            {
                Post = _mapper.Map<PostDto>(meesage),
                TargetedUserId = userId,
                MessageId = messageId
            };

            var message = JsonSerializer.Serialize(updateDto);

            var subscriber = _redisCacheClient.GetDbFromConfiguration().Database.Multiplexer.GetSubscriber();

            await subscriber.PublishAsync(RedisChannel.Literal(userId), message);
        }
    }
}
