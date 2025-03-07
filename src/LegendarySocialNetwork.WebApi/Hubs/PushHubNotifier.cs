using AutoMapper;
using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Domain.Messages;
using LegendarySocialNetwork.WebApi.DataClasses.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace LegendarySocialNetwork.WebApi.Hubs
{
    public class PushHubNotifier : IPushHubNotifier
    {
        private readonly IHubContext<FeedHub, IFeedClient> _hubContext;
        private readonly IMapper _mapper;

        public PushHubNotifier(IHubContext<FeedHub, IFeedClient> hubContext, IMapper mapper)
        {
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task PublishAsync(PostMessage meesage, string userId)
        {
            var post = _mapper.Map<PostDto>(meesage);
            await _hubContext.Clients.User(userId).PushToFeed(post);
        }
    }
}
