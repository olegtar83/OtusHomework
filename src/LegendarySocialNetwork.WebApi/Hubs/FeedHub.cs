using LegendarySocialNetwork.WebApi.DataClasses.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LegendarySocialNetwork.WebApi.Hubs
{
    [Authorize]
    public class FeedHub : Hub<IFeedClient>
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }

    public interface IFeedClient
    {
        Task PushToFeed(PostDto post);
    }
}
