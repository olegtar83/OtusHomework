using AutoMapper;
using LegendarySocialNetwork.Application.Common.Caching;
using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Domain.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LegendarySocialNetwork.Application.Consumers
{
    public class UpdateCacheFeedConsumer : IConsumer<UpdateFeedMessage>
    {
        private readonly IListCache<PostMessage> _listCache;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<UpdateCacheFeedConsumer> _logger;
        private readonly IMapper _mapper;

        public UpdateCacheFeedConsumer(IListCache<PostMessage> listCache,
             IPostRepository postRepository,
             ILogger<UpdateCacheFeedConsumer> logger,
             IMapper mapper)
        {
            _listCache = listCache;
            _postRepository = postRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<UpdateFeedMessage> context)
        {
            _logger.LogInformation("Kafka message arrived for post cache");

            if (context.Message.Operation == Operation.Reset)
            {
                var feedKey = CacheKeys.Feed.ForUser(context.Message.UserId!);
                var limitedPosts = await GetLimitedPostsAsync(context.Message.UserId!);
                await _listCache.ResetAsync(feedKey, limitedPosts);
                return;
            }

            foreach (var userId in context.Message.FriendsIds)
            {
                var feedKey = CacheKeys.Feed.ForUser(userId);
                var posts = _listCache.GetAsync(feedKey);

                switch (context.Message.Operation)
                {
                    case Operation.Create:
                        _logger.LogInformation($"Adding post {context.Message.Post!.Id} to user #{userId}");
                        if (posts.Count > 1000)
                        {
                            var limitedPosts = await GetLimitedPostsAsync(userId);
                            await _listCache.ResetAsync(feedKey, limitedPosts);
                        }
                        await _listCache.AddAsync(feedKey, context.Message.Post);
                        break;

                    case Operation.Update:
                        _logger.LogInformation($"Updating post {context.Message.Post!.Id} for user #{userId}");
                        if (posts.Any())
                        {
                            var post = posts.FirstOrDefault(x => x.Id == context.Message.Post.Id);
                            if (post != null)
                            {
                                post.Text = context.Message.Post.Text;
                            }
                        }
                        await _listCache.ResetAsync(feedKey, posts);
                        break;

                    case Operation.Delete:
                        _logger.LogInformation($"Deleting post {context.Message.Post!.Id} for user #{userId}");
                        if (posts.Any())
                        {
                            var post = posts.FirstOrDefault(x => x.Id == context.Message.Post.Id);
                            if (post != null)
                            {
                                posts.Remove(post);
                            }
                        }
                        await _listCache.ResetAsync(feedKey, posts);
                        break;

                    default:
                        _logger.LogWarning($"Unknown operation: {context.Message.Operation}");
                        break;
                }
            }
        }

        private async Task<List<PostMessage>> GetLimitedPostsAsync(string userId)
        {
            var res = await _postRepository.GetLimitedFeedAsync(userId);
            return _mapper.Map<List<PostMessage>>(res.Value); ;
        }
    }
}
