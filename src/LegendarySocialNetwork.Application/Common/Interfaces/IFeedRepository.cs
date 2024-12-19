using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;

public interface IFeedRepository
{
    Task<Result<List<PostEntity>>> GetFeedAsync(string userId);
}