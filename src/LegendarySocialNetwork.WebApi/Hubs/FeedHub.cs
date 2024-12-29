using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.WebApi.DataClasses.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LegendarySocialNetwork.WebApi.Hubs
{
    [Authorize]
    public class FeedHub : Hub<IFeedClient>
    {
        private readonly IPushHubNotifier _notifier;
        public FeedHub(IPushHubNotifier notifier)
        {
            _notifier = notifier;

        }
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User!.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            await _notifier.SubscribeToFeedAsync(userId!);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User!.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            await _notifier.UnsubscribeFromFeedAsync(userId!);

            await base.OnDisconnectedAsync(exception);
        }
    }

    public interface IFeedClient
    {
        Task PushToFeed(PostDto post);
    }
}
