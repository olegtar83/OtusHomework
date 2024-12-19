using LegendarySocialNetwork.Application.Common.Models;
using LegendarySocialNetwork.Domain.Entities;

namespace LegendarySocialNetwork.Application.Common.Interfaces
{
    public interface IAuthRepository
    {
        Task<Result<AccountEntity>> GetLoginAsync(string id);
        Task<Result<string>> RegisterAsync(UserEntity user, string password);
    }
}