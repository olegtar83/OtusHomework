using LegendarySocialNetwork.Database.Entities;
using LegendarySocialNetwork.DataClasses.Internals;

namespace LegendarySocialNetwork.Database;


public interface IDatabaseContext
{
    Task<Result<AccountEntity>> GetLoginAsync(string id);
    Task<Result<string>> RegisterAsync(UserEntity user, string password);
    Task<Result<UserEntity>> GetUserAsync(string id);
}
