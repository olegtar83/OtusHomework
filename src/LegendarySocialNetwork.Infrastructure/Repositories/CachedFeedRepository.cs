using AutoMapper;
using LegendarySocialNetwork.Application.Common.Caching;
using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Application.Events;
using LegendarySocialNetwork.Domain.Entities;
using LegendarySocialNetwork.Domain.Messages;
using MediatR;

public class CachedFeedRepository : IFeedRepository
{
    private readonly IFeedRepository _innerRepository;
    private readonly IListCache<PostMessage> _listCache;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public CachedFeedRepository(IFeedRepository innerRepository,
        IListCache<PostMessage> listCache,
        IMapper mapper,
        IPublisher publisher)
    {
        _innerRepository = innerRepository;
        _listCache = listCache;
        _mapper = mapper;
        _publisher = publisher;
    }

    public async Task<Result<List<PostEntity>>> GetFeedAsync(string userId)
    {
        var feedKey = CacheKeys.Feed.ForUser(userId);
        var posts = _listCache.GetAsync(feedKey);

        if (posts.Count > 0)
        {
            return Result<List<PostEntity>>.
                Success(_mapper.Map<List<PostEntity>>(posts));
        }

        var result = await _innerRepository.GetFeedAsync(userId);
        if (result.Succeeded)
        {
            await _publisher.Publish(new UpdateFeedEventRequested(new UpdateFeedMessage
            {
                UserId = userId,
                Operation = Operation.Reset
            }));
        }
        return result;
    }
}