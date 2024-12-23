using LegendarySocialNetwork.Messages.Database.Entities;
using LegendarySocialNetwork.Messages.DataClasses.Models;

namespace LegendarySocialNetwork.Messages.Database
{
    public interface IDatabaseContext
    {
        Task<Result<string>> SetDialogAsync(string from, string to, string text);
        Task<Result<List<DialogEntity>>> GetDialogsAsync(string id);
    }
}
