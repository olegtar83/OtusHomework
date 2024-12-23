using LegendarySocialNetwork.Messages.Database.Entities;
using LegendarySocialNetwork.Messages.DataClasses.Models;

namespace LegendarySocialNetwork.Messages.Database
{
    public interface IDatabaseContext
    {
        Task<Result<string>> SetDialogAsync(string id, string from, string to, string text, int shardId);
        Task<Result<List<DialogEntity>>> GetDialogsAsync(string id);
        Task<Result<string>> GetDialogIdAsync(string from, string to);
    }
}
