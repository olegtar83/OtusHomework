﻿using LegendarySocialNetwork.Messages.Database;
using LegendarySocialNetwork.Messages.DataClasses.Models;
using LegendarySocialNetwork.Messages.DataClasses.Responses;
using LegendarySocialNetwork.Messages.Tarantool;

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
        private readonly ITarantoolService _tarantoolService;
        public DialogService(IDatabaseContext databaseContext,
            ITarantoolService tarantoolService)
        {
            _databaseContext = databaseContext;
            _tarantoolService = tarantoolService;
        }

        public async Task<Result<string>> SetDialogAsync(string from, string text, string to)
        {

            return await _databaseContext.SetDialogAsync(from, text, to);
        }

        public async Task<Result<List<DialogResp>>> GetDialogsAsync(string userId)
        {
            var res = await _tarantoolService.GetDialogsAsync(userId);
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
