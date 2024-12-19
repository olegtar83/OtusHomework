using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;
using MediatR;

namespace LegendarySocialNetwork.Infrastructure.Repositories
{
    public interface IFriendshipRepository
    {
        Task<Result<Unit>> DeleteAsync(string requesterUserId, string addressedUserId);
        Task<Result<Unit>> SetAsync(string requesterUserId, string addressedUserId);
        Task<Result<IEnumerable<FriendshipEntity>>> GetAsync(string userId);

        Task<Result<List<FriendEntity>>> GetFriendsAsync(string user_id);
    }
}