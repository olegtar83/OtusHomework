using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;
using MediatR;

namespace LegendarySocialNetwork.Application.Common.Interfaces
{
    public interface IPostRepository
    {
        Task<Result<string>> CreateAsync(string text, string userId);
        Task<Result<Unit>> DeleteAsync(string postId);
        Task<Result<PostEntity>> GetAsync(string postId);
        Task<Result<Unit>> UpdateAsync(string text, string postId, string userId);
        Task<Result<List<PostEntity>>> GetLimitedFeedAsync(string user_id, int? limit = 1000);

    }
}