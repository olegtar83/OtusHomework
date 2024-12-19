using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;

namespace LegendarySocialNetwork.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<Result<UserEntity>> GetUserAsync(string id);
        Task<Result<List<UserEntity>>> SearchUserAsync(string? firstName, string? lastName);
    }
}