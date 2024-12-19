using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;

namespace LegendarySocialNetwork.Infrastructure.Repositories;

public class FeedRepository : IFeedRepository
{
    private readonly IPostRepository _postRepository;

    public FeedRepository(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<Result<List<PostEntity>>> GetFeedAsync(string userId)
    {
        return await _postRepository.GetLimitedFeedAsync(userId);
    }
}