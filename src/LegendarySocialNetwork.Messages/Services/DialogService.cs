using LegendarySocialNetwork.Messages.Database;
using LegendarySocialNetwork.Messages.DataClasses.Models;
using LegendarySocialNetwork.Messages.DataClasses.Responses;
using LegendarySocialNetwork.Messages.Utilities;

namespace LegendarySocialNetwork.Messages.Services
{
    public interface IDialogService
    {
        Task<Result<List<DialogResp>>> GetDialogsAsync(string userId);
        Task<Result<string>> SetDialogAsync(string from, string text, string to);
    }

    public class DialogService : IDialogService
    {
        private readonly IDatabaseContext _databaseContext;
        public DialogService(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Result<string>> SetDialogAsync(string from, string text, string to)
        {
            var resId = await _databaseContext.GetDialogIdAsync(from, to);

            string id = resId.Succeeded ? resId.Value : Guid.NewGuid().ToString();

            int shardId = HashUtility.HashIdToShard(id);

            return await _databaseContext.SetDialogAsync(id, from, text, to, shardId);
        }

        public async Task<Result<List<DialogResp>>> GetDialogsAsync(string userId)
        {
            var res = await _databaseContext.GetDialogsAsync(userId);
            if (res.Succeeded)
            {
                var dialogs = res.Value.Select(x => new DialogResp
                {
                    From = x.From,
                    Text = x.Text,
                    To = x.To
                }).ToList();

                return Result<List<DialogResp>>.Success(dialogs);
            }
            return Result<List<DialogResp>>.Failure(res.Error);
        }
    }
}
